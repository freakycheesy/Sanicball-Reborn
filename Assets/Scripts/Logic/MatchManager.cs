using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using Newtonsoft.Json;
using Sanicball.Data;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public class MatchManager : MonoBehaviour
    {
        #region Events

        public EventHandler<MatchPlayerEventArgs> MatchPlayerAdded;
        public EventHandler<MatchPlayerEventArgs> MatchPlayerRemoved;
        public EventHandler MatchSettingsChanged;

        #endregion Events

        #region Exposed fields

        public static MatchManager Instance;
        public SceneReference menuScene;
        public SceneReference lobbyScene;

        //Prefabs
        [SerializeField]
        public PauseMenu pauseMenuPrefab;
        [SerializeField]
        public Chat chatPrefab;
        [SerializeField]
        public RaceManager raceManagerPrefab;
        [SerializeField]
        public UI.Popup disconnectedPopupPrefab;
        [SerializeField]
        public Marker markerPrefab = null;

        #endregion Exposed fields

        #region Match state

        //List of all clients in the match. Only serves a purpose in online play.
        //In local play, this list will always only contain the local client.
        [SerializeField]
        public List<MatchClient> clients = new List<MatchClient>();

        //Holds the guid of the local client, to check if messages are directed at it.
        public NetworkConnectionToClient myGuid;

        //List of all players - players are seperate from clients because each client can have
        //up to 4 players playing in splitscreen.
        public List<MatchPlayer> players = new List<MatchPlayer>();

        //These settings will be used when starting a race
        [SerializeField] public MatchSettings currentSettings = MatchSettings.CreateDefault();

        //Lobby countdown timer stuff
        public bool lobbyTimerOn = false;
        public const float lobbyTimerMax = 3;
        public float lobbyTimer = lobbyTimerMax;

        //Auto start timer (Only used in online mode)
        public bool autoStartTimerOn = false;
        public float autoStartTimer = 0;

        #endregion Match state

        #region Scenes and scene initializing

        //Bools for scene initializing and related stuff
        public bool inLobby = false; //Are we in the lobby or in a race?
        public bool loadingLobby = false;
        public bool loadingStage = false;
        public bool joiningRaceInProgress = false; //If true, RaceManager will be created as if a race was already in progress.
        public bool showSettingsOnLobbyLoad = false; //If true, the match settings window will pop up when the lobby scene is entered.

        #endregion Scenes and scene initializing

        //Match messenger used to send and receive state changes.
        //This will be either a LocalMatchMessenger or OnlineMatchMessenger, but each are used the same way.

        public UI.Chat activeChat;

        //Timer used for syncing realtime stuff in online
        #region Properties
        /// <summary>
        /// Contains all clients connected to the game. In offline matches this will always only contain one client.
        /// </summary>
        public ReadOnlyCollection<MatchClient> Clients { get { return clients.AsReadOnly(); } }

        /// <summary>
        /// Contains all players in the game, even ones from other clients in online races
        /// </summary>
        public ReadOnlyCollection<MatchPlayer> Players { get { return players.AsReadOnly(); } }

        /// <summary>
        /// Current settings for this match. On remote clients, this is only used for showing settings on the UI.
        /// </summary>
        public MatchSettings CurrentSettings { get { return currentSettings; } set { currentSettings = value; } }

        public NetworkConnection LocalClientGuid { get { return myGuid; } }

        public bool AutoStartTimerOn { get { return autoStartTimerOn; } }
        public float AutoStartTimer { get { return autoStartTimer; } }
        public bool InLobby { get { return inLobby; } }

        #endregion Properties

        #region State changing methods
        [TextArea(1, 50)]
        public string matchSettingJson;

        [Server]
        public void RequestSettingsChange(MatchSettings newSettings)
        {
            LobbyScript.Instance.ChangeSettingsRpc(newSettings);
        }

        public void RequestPlayerJoin(ControlType ctrlType, int initialCharacter)
        {
            LobbyScript.Instance.PlayerJoinRpc(myGuid, ctrlType, initialCharacter);
        }

        public void RequestPlayerLeave(ControlType ctrlType)
        {
            LobbyScript.Instance.PlayerLeaveRpc(myGuid, ctrlType);
        }

        public void RequestCharacterChange(ControlType ctrlType, int newCharacter)
        {
            LobbyScript.Instance.CharacterChangedRpc(myGuid, ctrlType, newCharacter);
        }

        public void RequestReadyChange(ControlType ctrlType, bool ready)
        {
            LobbyScript.Instance.ChangedReadyServerRpc(myGuid, ctrlType, ready);
        }

        public void RequestLoadLobby()
        {
            LobbyScript.Instance.LoadLobbyRpc();
        }

        #endregion State changing methods

        #region Match message callbacks

        public void ClientLeftCallback(NetworkConnection guid)
        {
            //Remove all players added by this client
            List<MatchPlayer> playersToRemove = players.Where(a => a.ClientGuid == guid).ToList();
            foreach (MatchPlayer player in playersToRemove)
            {
                PlayerLeftCallback(player.ClientGuid, player.CtrlType);
            }
            //Remove the client
            clients.RemoveAll(a => a.Guid == guid);
        }
        public void PlayerLeftCallback(NetworkConnection guid, ControlType type)
        {
            var player = players.FirstOrDefault(a => a.ClientGuid == guid && a.CtrlType == type);
            if (player != null)
            {
                players.Remove(player);

                if (player.BallObject)
                {
                    player.BallObject.CreateRemovalParticles();
                    Destroy(player.BallObject.gameObject);
                }

                if (MatchPlayerRemoved != null)
                    MatchPlayerRemoved(this, new MatchPlayerEventArgs(player, guid == myGuid)); //TODO: determine if removed player was local
            }
        }

        public void CharacterChangedCallback(NetworkConnection myGuid, ControlType ctrlType, int newCharacter)
        {
            if (!inLobby)
            {
                Debug.LogError("Cannot set character outside of lobby!");
            }

            var player = players.FirstOrDefault(a => a.ClientGuid == myGuid && a.CtrlType == ctrlType);
            if (player != null)
            {
                player.CharacterId = newCharacter;
                SpawnLobbyBall(player);
            }
        }

        public void ChangedReadyCallback(NetworkConnection myGuid, ControlType ctrlType)
        {
            var player = players.FirstOrDefault(a => a.ClientGuid == myGuid && a.CtrlType == ctrlType);
            if (player != null)
            {
                player.ReadyToRace = !player.ReadyToRace;

                //Check if all players are ready and start/stop lobby timer accordingly
                var allReady = players.TrueForAll(a => a.ReadyToRace);
                if (allReady && !lobbyTimerOn)
                {
                    Debug.Log("Start Lobby Timer");
                    StartLobbyTimer(0);
                }
                if (!allReady && lobbyTimerOn)
                {
                    Debug.Log("Stop Lobby Timer");
                    StopLobbyTimer();
                }
            }
        }

        public void LoadRaceCallback()
        {
            StopLobbyTimer();
            CameraFade.StartAlphaFade(Color.black, false, 0.3f, 0.05f, GoToStage);
        }

        public void ChatCallback(string from, string text)
        {
            if (activeChat)
                activeChat.ShowMessage(from, text);
        }

        public void LoadLobbyCallback()
        {
            GoToLobby();
        }

        public void AutoStartTimerCallback(bool Enabled)
        {
            autoStartTimerOn = Enabled;
            autoStartTimer = currentSettings.AutoStartTime - (float)NetworkTime.rtt;
        }

        #endregion Match message callbacks

        #region Match initializing

        public void InitMatch()
        {
            CreateLobby();
        }

        public static void CreateLobby()
        {
            Instance.currentSettings = ActiveData.MatchSettings;
            AddressableNetworkManager.AddressableManager.StartHost();
        }

        public static void JoinLobby(string ip)
        {
            Instance.showSettingsOnLobbyLoad = false;
            AddressableNetworkManager.AddressableManager.networkAddress = ip;
            AddressableNetworkManager.AddressableManager.StartHost();
        }

        public void LeaveLobby()
        {
            inLobby = false;
            loadingLobby = false;
            NetworkManager.singleton.StopHost();
            Destroy(this.gameObject);
        }

        #endregion Match initializing
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public LobbyScript LobbyPrefab;
        public void Start()
        {
            if (Instance != this) return;
            Instance = this;
            SceneManager.sceneLoaded += OnLevelHasLoaded;
            DontDestroyOnLoad(gameObject);
        }

        public void LocalChatMessageSent(string from, string text)
        {
            MatchClient myClient = clients.FirstOrDefault(a => a.Guid == myGuid);
            ChatCallback(from, text);
        }
        /*
                public void OnlinePlayerMovement(object sender, PlayerMovementArgs e)
                {
                    MatchPlayer player = players.FirstOrDefault(a => a.ClientGuid == e.Movement.ClientGuid && a.CtrlType == e.Movement.CtrlType);
                    if (player != null && player.BallObject != null)
                    {
                        player.ProcessMovement(e.Timestamp, e.Movement);
                    }
                }
        */
        public void Update()
        {
            //var messenger = InstanceFinder.ClientManager;
            matchSettingJson = JsonConvert.SerializeObject(currentSettings);
            //Pausing/unpausing
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7))
            {
                if (!UI.PauseMenu.GamePaused)
                {
                    UI.PauseMenu menu = Instantiate(pauseMenuPrefab);
                }
                else
                {
                    var menu = PauseMenu.Instance;
                    if (menu)
                        Destroy(menu.gameObject);
                }
            }

            if (lobbyTimerOn && inLobby)
            {
                lobbyTimer -= Time.deltaTime;
                LobbyReferences.Active.CountdownField.text = "Match starts in " + Mathf.Max(1f, Mathf.Ceil(lobbyTimer));

                //LoadRaceMessages don't need to be sent in online mode - the server will ignore it anyway.
                if (lobbyTimer <= 0)
                {
                    LobbyScript.Instance.LoadRaceRpc();
                }
            }

            if (autoStartTimerOn && inLobby)
            {
                autoStartTimer = Mathf.Max(0, autoStartTimer - Time.deltaTime);
            }
        }

        public void OnDestroy()
        {
            if (activeChat)
                Destroy(activeChat.gameObject);
        }

        #region Players ready and lobby timer

        public void StartLobbyTimer(float offset = 0)
        {
            lobbyTimerOn = true;
            lobbyTimer -= offset;
            LobbyReferences.Active.CountdownField.enabled = true;
        }

        public void StopLobbyTimer()
        {
            lobbyTimerOn = false;
            lobbyTimer = lobbyTimerMax;
            LobbyReferences.Active.CountdownField.enabled = false;
        }

        #endregion Players ready and lobby timer

        #region Scene changing / race loading

        [Server]
        public void GoToLobby()
        {
            if (inLobby) return;

            loadingStage = false;
            loadingLobby = true;
            if(BootstrapSceneManager.currentScene.Scene != null) BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
            else BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
        }
        [Server]
        public void GoToStage()
        {
            CurrentStage = ActiveData.GetStageByBarcode(currentSettings.StageBarcode);

            loadingStage = true;
            loadingLobby = false;

            foreach (var p in Players)
            {
                p.ReadyToRace = false;
            }

            ActiveData.LoadLevel(CurrentStage);
        }

        public static StageInfo CurrentStage;

        //Check if we were loading the lobby or the race
        public void OnLevelHasLoaded(Scene scene, LoadSceneMode mode)
        {
            if (loadingLobby)
            {
                InitLobby();
                loadingLobby = false;
            }
            if (loadingStage)
            {
                InitRace();
                loadingStage = false;
            }
            foreach (var camera in Resources.FindObjectsOfTypeAll<UniversalAdditionalCameraData>())
            {
                camera.renderPostProcessing = true;
            }
        }

        //Initiate the lobby after loading lobby scene
        public void InitLobby()
        {
            inLobby = true;

            foreach (var p in Players)
            {
                SpawnLobbyBall(p);
            }

            if (showSettingsOnLobbyLoad)
            {
                //Let the player pick settings first time entering the lobby
                LobbyReferences.Active.MatchSettingsPanel.Show();
                showSettingsOnLobbyLoad = false;
            }
        }

        //Initiate a race after loading the stage scene
        public void InitRace()
        {
            inLobby = false;

            var raceManager = Instantiate(raceManagerPrefab);
            raceManager.Init(currentSettings, this, joiningRaceInProgress);
            joiningRaceInProgress = false;
        }

        public void QuitMatch(string reason = null)
        {
            StartCoroutine(QuitMatchInternal(reason));
        }

        public IEnumerator QuitMatchInternal(string reason)
        {
            LeaveLobby();

            if (reason != null)
            {
                yield return null;

                FindAnyObjectByType<UI.PopupHandler>().OpenPopup(disconnectedPopupPrefab);
                FindAnyObjectByType<UI.PopupDisconnected>().Reason = reason;
            }

            Addressables.LoadSceneAsync(menuScene);
        }

        #endregion Scene changing / race loading

        public void SpawnLobbyBall(MatchPlayer player)
        {
            var spawner = LobbyReferences.Active.BallSpawner;
            if (player.BallObject != null)
            {
                player.BallObject.CreateRemovalParticles();
                Destroy(player.BallObject.gameObject);
            }

            string name = clients.First(a => a.Guid == player.ClientGuid).Name + " (" + GameInput.GetControlTypeName(player.CtrlType) + ")";

            player.BallObject = spawner.SpawnBall(PlayerType.Normal, (player.ClientGuid == myGuid) ? player.CtrlType : ControlType.None, player.CharacterId, name, player.ClientGuid);

            if (player.ClientGuid != myGuid)
            {
                Marker marker = Instantiate(markerPrefab);
                marker.transform.SetParent(LobbyReferences.Active.MarkerContainer, false);
                marker.Color = Color.clear;
                marker.Text = name;
                marker.Target = player.BallObject.transform;
            }
        }
    }
}