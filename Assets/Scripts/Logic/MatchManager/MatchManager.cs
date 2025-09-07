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
            Instance.CurrentSettings = newSettings;
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
            NetworkServer.SendToAll<LoadLobbyMessage>(new());
        }
       
        void Start()
        {
            Instance = this;
            SceneManager.sceneLoaded += OnLevelHasLoaded;
            DontDestroyOnLoad(gameObject);
            RegisterNetworkMessages();
            activeChat = Instantiate(chatPrefab);
            activeChat.MessageSent += LocalChatMessageSent;

            MatchManagerSpawned?.Invoke(this, Time.time);
        }
        public override void OnStopServer()
        {
            if (MatchManager.Instance)
            {
                MatchManager.Instance.inLobby = false;
                MatchManager.Instance.loadingLobby = false;
            }
        }
        public override void OnStartServer()
        {
            DontDestroyOnLoad(gameObject);
            CurrentSettings = ActiveData.MatchSettings;
            showSettingsOnLobbyLoad = true;
            if (AddressablesNetworkManager.networkSceneName.Contains("Lobby"))
            {
                inLobby = true;
            }
        }
        public override void OnStartClient()
        {
            Instance = this;
            if (isClientOnly) MatchManager.Instance.showSettingsOnLobbyLoad = false;
            RegisterNetworkMessages();
            NetworkClient.Send<ClientJoinedMessage>(new(ActiveData.GameSettings.nickname));
        }
        private const bool RequireAuth = true;
        private void RegisterNetworkMessages()
        {
            NetworkServer.RegisterHandler<AutoStartTimerMessage>((_, a)=>AutoStartTimerCallback(a), RequireAuth);
            NetworkServer.RegisterHandler<ChangedReadyMessage>(ChangedReadyCallback, RequireAuth);
            NetworkServer.RegisterHandler<CharacterChangedMessage>(CharacterChangedCallback, RequireAuth);
            NetworkServer.RegisterHandler<ChatMessage>((_, a)=>ChatCallback(a), RequireAuth);
            NetworkServer.RegisterHandler<ClientJoinedMessage>(ClientJoinedCallback, RequireAuth);
            NetworkServer.RegisterHandler<ClientLeftMessage>(ClientLeftCallback, RequireAuth);
            NetworkServer.RegisterHandler<PlayerJoinedMessage>(PlayerJoinedCallback, RequireAuth);
            NetworkServer.RegisterHandler<PlayerLeftMessage>(PlayerLeftCallback, RequireAuth);
            NetworkClient.RegisterHandler<SettingsChangedMessage>(SettingsChangedCallback);
            NetworkClient.RegisterHandler<LoadRaceMessage>((_, _)=>LoadRaceCallback());
            NetworkClient.RegisterHandler<LoadLobbyMessage>((_, _) => LoadLobbyCallback());
        }

        private void PlayerJoinedCallback(NetworkConnectionToClient conn, PlayerJoinedMessage message)
        {
            var p = new MatchPlayer(conn.connectionId, message.CtrlType, message.InitialCharacter);
            MatchManager.Instance.Players.Add(p);

            if (MatchManager.Instance.inLobby)
            {
                MatchManager.Instance.SpawnLobbyBall(conn,p);
            }

            MatchManager.Instance.StopLobbyTimer();

            MatchManager.MatchPlayerAdded(this, new MatchPlayerEventArgs(p, conn.identity.isLocalPlayer));
        }

        private void ClientJoinedCallback(NetworkConnectionToClient conn, ClientJoinedMessage message)
        {
            var matchClient = new MatchClient(conn.connectionId, message.ClientName);
            if (MatchManager.Instance.Clients.Contains(matchClient)) return;
            MatchManager.Instance.Clients.Add(matchClient);
            Debug.Log("New client " + message.ClientName);
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
            matchSettingJson = JsonConvert.SerializeObject(CurrentSettings);
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

        public void SpawnLobbyBall(NetworkConnectionToClient conn, MatchPlayer player)
        {
            var spawner = LobbyReferences.Active.BallSpawner;
            if (player.BallObject != null)
            {
                player.BallObject.CreateRemovalParticles();
                Destroy(player.BallObject.gameObject);
            }

            string name = Clients.First(a => a.ConnectionId == player.ConnectionId).Name + " (" + GameInput.GetControlTypeName(player.CtrlType) + ")";
            player.BallObject = spawner.SpawnBall((player.ConnectionId == connectionToClient.connectionId) ? player.CtrlType : ControlType.None, player.CharacterId, name, conn);

            if (player.ConnectionId != connectionToClient.connectionId)
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