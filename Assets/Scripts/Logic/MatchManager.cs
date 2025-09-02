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
using SanicballCore.MatchMessages;
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
        public NetworkConnectionToClient myGuid => NetworkServer.localConnection;

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

        public int LocalClientGuid { get { return myGuid.connectionId; } }

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
            SettingsChangedMessage message = new(newSettings);
            NetworkClient.Send(message);
        }

        public void RequestPlayerJoin(ControlType ctrlType, int initialCharacter)
        {
            NetworkClient.Send<PlayerJoinedMessage>(new(LocalClientGuid, ctrlType, initialCharacter));
        }

        public void RequestPlayerLeave(ControlType ctrlType)
        {
            NetworkClient.Send<PlayerLeftMessage>(new(LocalClientGuid, ctrlType));
        }

        public void RequestCharacterChange(ControlType ctrlType, int newCharacter)
        {
            NetworkClient.Send<CharacterChangedMessage>(new(LocalClientGuid, ctrlType, newCharacter));
        }

        public void RequestReadyChange(ControlType ctrlType, bool ready)
        {
            NetworkClient.Send<ChangedReadyMessage>(new(LocalClientGuid, ctrlType, ready));
        }

        public void RequestLoadLobby()
        {
            NetworkServer.SendToAll<LoadLobbyMessage>(new());
        }

        #endregion State changing methods

        #region Match message callbacks

        public void ClientLeftCallback(ClientLeftMessage message)
        {
            //Remove all players added by this client
            List<MatchPlayer> playersToRemove = players.Where(a => a.ConnectionId == message.ConnectionID).ToList();
            foreach (MatchPlayer player in playersToRemove)
            {
                PlayerLeftCallback(new(player.ConnectionId, player.CtrlType));
            }
            //Remove the client
            clients.RemoveAll(a => a.ConnectionId == message.ConnectionID);
        }
        public void PlayerLeftCallback(PlayerLeftMessage message)
        {
            int guid = message.ConnectionID;
            ControlType type = message.CtrlType;
            var player = players.FirstOrDefault(a => a.ConnectionId == guid && a.CtrlType == type);
            if (player != null)
            {
                players.Remove(player);

                if (player.BallObject)
                {
                    player.BallObject.CreateRemovalParticles();
                    NetworkServer.Destroy(player.BallObject.gameObject);
                }

                if (MatchPlayerRemoved != null)
                    MatchPlayerRemoved(this, new MatchPlayerEventArgs(player, guid == myGuid.connectionId)); //TODO: determine if removed player was local
            }
        }

        public void CharacterChangedCallback(CharacterChangedMessage message)
        {
            if (!inLobby)
            {
                Debug.LogError("Cannot set character outside of lobby!");
            }

            var player = players.FirstOrDefault(a => a.ConnectionId == message.ConnectionID && a.CtrlType == message.CtrlType);
            if (player != null)
            {
                player.CharacterId = message.NewCharacter;
                SpawnLobbyBall(player);
            }
        }

        public void ChangedReadyCallback(ChangedReadyMessage message)
        {
            var player = Instance.players.FirstOrDefault(a => a.ConnectionId == message.ConnectionID && a.CtrlType == message.CtrlType);
            if (player != null)
            {
                player.ReadyToRace = !player.ReadyToRace;
            }
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

        public void LoadRaceCallback()
        {
            StopLobbyTimer();
            CameraFade.StartAlphaFade(Color.black, false, 0.3f, 0.05f, GoToStage);
        }

        public void ChatCallback(ChatMessage message)
        {
            if (activeChat)
                activeChat.ShowMessage(message.From, message.Text);
        }

        public void LoadLobbyCallback()
        {
            GoToLobby();
        }

        public void AutoStartTimerCallback(AutoStartTimerMessage message)
        {
            autoStartTimerOn = message.Enabled;
            autoStartTimer = currentSettings.AutoStartTime - (float)NetworkTime.rtt;
        }

        #endregion Match message callbacks

        #region Match initializing

        public void InitMatch()
        {
            CreateLobby();
        }

        public void CreateLobby()
        {
            NetworkManager.singleton?.StartHost();
        }

        public void JoinLobby(string ip)
        {
            NetworkManager.singleton.networkAddress = ip;
            NetworkManager.singleton?.StartClient();
        }

        public void LeaveLobby()
        {
            NetworkManager.singleton?.StopHost();
        }

        #endregion Match initializing
        void Awake()
        {
            if (!Instance)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        public void Start()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnLevelHasLoaded;
            DontDestroyOnLoad(gameObject);
            RegisterNetworkMessages();
        }

        private void RegisterNetworkMessages()
        {
            NetworkClient.RegisterHandler<AutoStartTimerMessage>(AutoStartTimerCallback);
            NetworkClient.RegisterHandler<ChangedReadyMessage>(ChangedReadyCallback);
            NetworkClient.RegisterHandler<CharacterChangedMessage>(CharacterChangedCallback);
            NetworkClient.RegisterHandler<ChatMessage>(ChatCallback);
            NetworkClient.RegisterHandler<ClientJoinedMessage>(ClientJoinedCallback);
            NetworkClient.RegisterHandler<ClientLeftMessage>(ClientLeftCallback);
            NetworkClient.RegisterHandler<PlayerJoinedMessage>(PlayerJoinedCallback);
            NetworkClient.RegisterHandler<PlayerLeftMessage>(PlayerLeftCallback);
            NetworkServer.RegisterHandler<SettingsChangedMessage>((_, a)=>SettingsChangedCallback(a));
            NetworkServer.RegisterHandler<LoadRaceMessage>((_, _)=>LoadRaceCallback());
            NetworkServer.RegisterHandler<LoadLobbyMessage>((_, _) => LoadLobbyCallback());
        }

        private void PlayerJoinedCallback(PlayerJoinedMessage message)
        {
            var p = new MatchPlayer(message.ConnectionID, message.CtrlType, message.InitialCharacter);
            MatchManager.Instance.players.Add(p);

            if (MatchManager.Instance.inLobby)
            {
                MatchManager.Instance.SpawnLobbyBall(p);
            }

            MatchManager.Instance.StopLobbyTimer();

            MatchManager.Instance.MatchPlayerAdded(this, new MatchPlayerEventArgs(p, message.ConnectionID == MatchManager.Instance.myGuid.connectionId));
        }

        private void ClientJoinedCallback(ClientJoinedMessage message)
        {
            var matchClient = new MatchClient(message.ConnectionID, message.ClientName);
            if (MatchManager.Instance.Clients.Contains(matchClient)) return;
            MatchManager.Instance.clients.Add(matchClient);
            Debug.Log("New client " + message.ClientName);
        }

        private void SettingsChangedCallback(SettingsChangedMessage message)
        {
            Instance.CurrentSettings = message.NewMatchSettings;
            Debug.Log("Settings changed");
            Instance.MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LocalChatMessageSent(string from, string text)
        {
            MatchClient myClient = clients.FirstOrDefault(a => a.ConnectionId == myGuid.connectionId);
            ChatCallback(new(from, ChatMessageType.User, text));
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
                    NetworkServer.SendToAll<LoadRaceMessage>(new());
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
            //if(BootstrapSceneManager.Scene != null) BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
            //else
                BootstrapSceneManager.LoadScene(lobbyScene.RuntimeKey);
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

            string name = clients.First(a => a.ConnectionId == player.ConnectionId).Name + " (" + GameInput.GetControlTypeName(player.CtrlType) + ")";
            NetworkServer.connections.TryGetValue(player.ConnectionId, out var conn);
            player.BallObject = spawner.SpawnBall(PlayerType.Normal, (player.ConnectionId == myGuid.connectionId) ? player.CtrlType : ControlType.None, player.CharacterId, name, conn);

            if (player.ConnectionId != myGuid.connectionId)
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