using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Newtonsoft.Json;
using Sanicball.Data;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public partial class MatchManager : NetworkBehaviour
    {
        [Server]
        public void RequestSettingsChange(MatchSettings newSettings)
        {
            SettingsChangedMessage message = new(newSettings);
            Instance.state.CurrentSettings = newSettings;
            MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
            NetworkClient.Send(message);
        }

        public void RequestPlayerJoin(ControlType ctrlType, int initialCharacter)
        {
            PlayerJoinedMessage message = new(ctrlType, initialCharacter);
            NetworkClient.Send(message);
        }

        public void RequestPlayerLeave(ControlType ctrlType)
        {
            PlayerLeftMessage message = new(ctrlType);
            NetworkClient.Send(message);
        }

        public void RequestCharacterChange(ControlType ctrlType, int newCharacter)
        {
            CharacterChangedMessage message = new(ctrlType, newCharacter);
            NetworkClient.Send(message);
        }

        public void RequestReadyChange(ControlType ctrlType, bool ready)
        {
            ChangedReadyMessage message = new(ctrlType, ready);
            NetworkClient.Send(message);
        }

        public void RequestLoadLobby()
        {
            NetworkClient.Send<LoadLobbyMessage>(new());
        }
        void Awake()
        {
            if (Instance != this && Instance)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
        }
        void Start()
        {
            Instance = this;
            state = new(ActiveData.MatchSettings);
            DontDestroyOnLoad(gameObject);
            if (NetworkServer.active) state.CurrentSettings = ActiveData.MatchSettings;
            if (NetworkServer.active) state.inLobby = true;
            if (NetworkServer.activeHost) state.showSettingsOnLobbyLoad = true;
            SceneManager.sceneLoaded += OnLevelHasLoaded;
            activeChat = Instantiate(chatPrefab);
            activeChat.MessageSent += LocalChatMessageSent;
            RegisterNetworkMessages();
            MatchManagerSpawned?.Invoke(this, Time.time);
        }
        
        public static bool IsLocalId(NetworkConnectionToClient id)
        {
            return IsLocalId(id.connectionId);
        }
        public static bool IsLocalId(int id)
        {
            return NetworkServer.localConnection.connectionId == id;
        }
        private const bool RequireAuth = true;
        private void RegisterNetworkMessages()
        {
            NetworkServer.ReplaceHandler<AutoStartTimerMessage>((_, a) => AutoStartTimerCallback(a), RequireAuth);
            NetworkServer.ReplaceHandler<ChangedReadyMessage>(ChangedReadyCallback, RequireAuth);
            NetworkServer.ReplaceHandler<CharacterChangedMessage>(CharacterChangedCallback, RequireAuth);
            NetworkServer.ReplaceHandler<ChatMessage>((_, a) => ChatCallback(a), RequireAuth);
            NetworkServer.ReplaceHandler<ClientJoinedMessage>(ClientJoinedCallback, RequireAuth);
            NetworkServer.ReplaceHandler<ClientLeftMessage>(ClientLeftCallback, RequireAuth);
            NetworkServer.ReplaceHandler<PlayerJoinedMessage>(PlayerJoinedCallback, RequireAuth);
            NetworkServer.ReplaceHandler<PlayerLeftMessage>(PlayerLeftCallback, RequireAuth);
            NetworkServer.ReplaceHandler<LoadLobbyMessage>(LoadLobbyCallback);
            NetworkClient.ReplaceHandler<SettingsChangedMessage>(SettingsChangedCallback);
            NetworkClient.ReplaceHandler<LoadRaceMessage>((_, _) => LoadRaceCallback());
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            localClient = new(NetworkServer.localConnection.connectionId, ActiveData.GameSettings.nickname);
            NetworkClient.Send<ClientJoinedMessage>(new(localClient));
        }

        private void PlayerJoinedCallback(NetworkConnectionToClient conn, PlayerJoinedMessage message)
        {
            var p = new MatchPlayer(conn.connectionId, message.CtrlType, message.InitialCharacter);
            if (state.players.Contains(p)) return;
            state.players.Add(p);

            if (state.inLobby)
            {
                SpawnLobbyBall(conn, p);
            }

            StopLobbyTimer();

            MatchPlayerAdded(this, new MatchPlayerEventArgs(p, conn.identity.isLocalPlayer));
        }

        private void ClientJoinedCallback(NetworkConnectionToClient conn, ClientJoinedMessage message)
        {
            var matchClient = message.Client;
            if (state.clients.Contains(matchClient)) return;
            if (MatchManager.Instance.Clients.Contains(matchClient)) return;
            MatchManager.Instance.Clients.Add(matchClient);
            Debug.Log("New client " + matchClient.Name);
        }

        private void SettingsChangedCallback(SettingsChangedMessage message)
        {
            Debug.Log("Settings changed");
            MatchSettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void LocalChatMessageSent(string from, string text)
        {
            ChatCallback(new(from, ChatMessageType.User, text));
        }

        public void Update()
        {
            //var messenger = InstanceFinder.ClientManager;
            matchSettingJson = JsonConvert.SerializeObject(state.CurrentSettings);
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

            if (state.lobbyTimerOn && state.inLobby)
            {
                state.lobbyTimer -= Time.deltaTime;
                LobbyReferences.Active.CountdownField.text = "Match starts in " + Mathf.Max(1f, Mathf.Ceil(state.lobbyTimer));

                //LoadRaceMessages don't need to be sent in online mode - the server will ignore it anyway.
                if (state.lobbyTimer <= 0)
                {
                    NetworkServer.SendToAll<LoadRaceMessage>(new());
                }
            }

            if (state.autoStartTimerOn && state.inLobby)
            {
                state.autoStartTimer = Mathf.Max(0, state.autoStartTimer - Time.deltaTime);
            }
        }

        public void OnDestroy()
        {
            if (activeChat)
                Destroy(activeChat.gameObject);
            state.inLobby = false;
            state.loadingLobby = false;
        }

        #region Players ready and lobby timer

        public void StartLobbyTimer(float offset = 0)
        {
            state.lobbyTimerOn = true;
            state.lobbyTimer -= offset;
            LobbyReferences.Active.CountdownField.enabled = true;
        }

        protected virtual void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                FixedServerUpdate();
            }
            if (NetworkServer.activeHost)
            {
                FixedHostUpdate();
            }
            if (NetworkClient.active)
            {
                FixedClientUpdate();
            }
        }

        protected virtual void FixedServerUpdate()
        {

        }

        protected virtual void FixedHostUpdate()
        {

        }
        protected virtual void FixedClientUpdate()
        {

        }

        public void StopLobbyTimer()
        {
            state.lobbyTimerOn = false;
            state.lobbyTimer = MatchState.lobbyTimerMax;
            LobbyReferences.Active.CountdownField.enabled = false;
        }

        #endregion Players ready and lobby timer

        public void SpawnLobbyBall(NetworkConnectionToClient conn, MatchPlayer player)
        {
            var spawner = LobbyReferences.Active.BallSpawner;
            if (player.BallObject != null)
            {
                player.BallObject.CreateRemovalParticles();
                Destroy(player.BallObject.gameObject);
            }

            string name = Clients.First(a => a.ConnectionId == conn.connectionId).Name + " (" + GameInput.GetControlTypeName(player.CtrlType) + ")";
            player.BallObject = spawner.SpawnBall((conn == NetworkServer.localConnection) ? player.CtrlType : ControlType.None, player.CharacterId, name, conn);

            if (conn != NetworkServer.localConnection)
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