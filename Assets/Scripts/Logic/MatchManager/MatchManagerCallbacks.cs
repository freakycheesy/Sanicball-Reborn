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
        public void ClientLeftCallback(NetworkConnectionToClient conn, ClientLeftMessage message)
        {
            //Remove all players added by this client
            List<MatchPlayer> playersToRemove = Players.Where(a => a.ConnectionId == conn.connectionId).ToList();
            foreach (MatchPlayer player in playersToRemove)
            {
                PlayerLeftCallback(conn, new(player.CtrlType));
            }
            //Remove the client
            Clients.RemoveAll(a => a.ConnectionId == conn.connectionId);
        }
        public void PlayerLeftCallback(NetworkConnectionToClient conn, PlayerLeftMessage message)
        {
            int guid = conn.connectionId;
            ControlType type = message.CtrlType;
            var player = Players.FirstOrDefault(a => a.ConnectionId == guid && a.CtrlType == type);
            if (player != null)
            {
                Players.Remove(player);

                if (player.BallObject)
                {
                    player.BallObject.CreateRemovalParticles();
                    NetworkServer.Destroy(player.BallObject.gameObject);
                }

                if (MatchPlayerRemoved != null)
                    MatchPlayerRemoved(this, new MatchPlayerEventArgs(player, conn.identity.isLocalPlayer)); //TODO: determine if removed player was local
            }
        }

        public void CharacterChangedCallback(NetworkConnectionToClient conn, CharacterChangedMessage message)
        {
            if (!inLobby)
            {
                Debug.LogError("Cannot set character outside of lobby!");
            }

            var player = Players.FirstOrDefault(a => a.ConnectionId == conn.connectionId && a.CtrlType == message.CtrlType);
            if (player != null)
            {
                player.CharacterId = message.NewCharacter;
                SpawnLobbyBall(conn, player);
            }
        }

        public void ChangedReadyCallback(NetworkConnectionToClient conn, ChangedReadyMessage message)
        {
            var player = Instance.Players.FirstOrDefault(a => a.ConnectionId == conn.connectionId && a.CtrlType == message.CtrlType);
            if (player != null)
            {
                player.ReadyToRace = !player.ReadyToRace;
            }
            //Check if all players are ready and start/stop lobby timer accordingly
            var allReady = Players.ToList().TrueForAll(a => a.ReadyToRace);
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
            autoStartTimer = CurrentSettings.AutoStartTime - (float)NetworkTime.rtt;
        }

#endregion Match message callbacks
    }
}
