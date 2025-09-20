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
        public void ClientJoinedCallback(NetworkConnectionToClient client)
        {
            MatchClient matchClient = new(client, $"Player{client.authenticationData}");
            Clients.Add(matchClient);
            Debug.Log("New client " + matchClient.Name);
            MatchManagerUpdated(this, new());
        }

        public void ClientLeftCallback(NetworkConnectionToClient conn)
        {
            //Remove all players added by this client
            List<MatchPlayer> playersToRemove = Players.Where(a => a.ConnectionId == conn).ToList();
            foreach (MatchPlayer player in playersToRemove)
            {
                PlayerLeftCallback(conn, new(player.CtrlType));
            }
            //Remove the client
            MatchManagerUpdated(this, new());
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
                MatchPlayerRemoved(this,new MatchPlayerEventArgs(player, conn.identity.isLocalPlayer)); //TODO: determine if removed player was local
            MatchManagerUpdated(this, new());
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

        public void LoadLobbyCallback(LoadLobbyMessage message)
        {
            GoToLobby();
        }

        public void AutoStartTimerCallback(AutoStartTimerMessage message)
        {
            state.autoStartTimerOn = message.Enabled;
            state.autoStartTimer = state.CurrentSettings.AutoStartTime - (float)NetworkTime.rtt;
        }

#endregion Match message callbacks
    }
}
