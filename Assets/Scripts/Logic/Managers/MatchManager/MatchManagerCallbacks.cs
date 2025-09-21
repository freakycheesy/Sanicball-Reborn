using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public partial class MatchManager : NetworkBehaviour
    {
        #region Match message callbacks

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
            MatchManagerUpdated(this, new());
            UpdateMatchManager();
        }

        public void ClientJoinedCallback(MatchClient client)
        {
            MatchManagerUpdated(this, new());
            UpdateMatchManager();
        }

        public void ClientLeftCallback(MatchClient client)
        {
            //Remove all players added by this client
            List<MatchPlayer> playersToRemove = Players.Where(a => a.ConnectionId == client.GetConnection()).ToList();
            foreach (MatchPlayer player in playersToRemove)
            {
                PlayerLeftCallback(client.GetConnection(), new(player.CtrlType));
            }
            //Remove the client
            MatchManagerUpdated(this, new());
            UpdateMatchManager();
        }
        public void PlayerLeftCallback(NetworkConnectionToClient conn, PlayerLeftMessage message)
        {
            ControlType type = message.CtrlType;
            var player = Players.FirstOrDefault(a => a.ConnectionId == conn && a.CtrlType == type);
            Players.Remove(player);

            if (player.BallObject)
            {
                player.BallObject.CreateRemovalParticles();
                NetworkServer.Destroy(player.BallObject.gameObject);
            }

            if (MatchPlayerRemoved != null)
                MatchPlayerRemoved(this, new MatchPlayerEventArgs(player, conn.identity.isLocalPlayer)); //TODO: determine if removed player was local
            MatchManagerUpdated(this, new());
            UpdateMatchManager();
        }

        public void CharacterChangedCallback(NetworkConnectionToClient conn, CharacterChangedMessage message)
        {
            if (!state.inLobby)
            {
                Debug.LogError("Cannot set character outside of lobby!");
            }

            var player = Players.FirstOrDefault(a => a.ConnectionId == conn && a.CtrlType == message.CtrlType);
            player.CharacterId = message.NewCharacter;
            SpawnLobbyBall(conn, player);
            UpdateMatchManager();
        }

        public void ChangedReadyCallback(NetworkConnectionToClient conn, ChangedReadyMessage message)
        {
            var player = Instance.Players.FirstOrDefault(a => a.ConnectionId == conn && a.CtrlType == message.CtrlType);
            player.ReadyToRace = !player.ReadyToRace;

            //Check if all players are ready and start/stop lobby timer accordingly
            var allReady = Players.ToList().TrueForAll(a => a.ReadyToRace);
            if (allReady && !state.lobbyTimerOn)
            {
                Debug.Log("Start Lobby Timer");
                StartLobbyTimer(0);
            }
            if (!allReady && state.lobbyTimerOn)
            {
                Debug.Log("Stop Lobby Timer");
                StopLobbyTimer();
            }
            UpdateMatchManager();
        }

        public void LoadRaceCallback()
        {
            StopLobbyTimer();
            CameraFade.StartAlphaFade(Color.black, false, 0.3f, 0.05f, GoToStage);
            UpdateMatchManager();
        }

        public void ChatCallback(ChatMessage message)
        {
            if (activeChat)
                activeChat.ShowMessage(message.From, message.Text);
            UpdateMatchManager();
        }

        public void LoadLobbyCallback(LoadLobbyMessage message)
        {
            GoToLobby();
            UpdateMatchManager();
        }

        public void AutoStartTimerCallback(AutoStartTimerMessage message)
        {
            state.autoStartTimerOn = message.Enabled;
            state.autoStartTimer = state.CurrentSettings.AutoStartTime - (float)NetworkTime.rtt;
            UpdateMatchManager();
        }

        #endregion Match message callbacks
    }
}
