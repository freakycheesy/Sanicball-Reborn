using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Sanicball.Data;
using Sanicball.Gameplay;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;

namespace Sanicball.Logic
{
    public partial class RaceManager : MonoBehaviour
    {
        [Server]
        public void StartRaceCallback()
        {
            countdownOffset = (float)NetworkTime.rtt;
            CurrentState = RaceState.Countdown;
        }
        public void ClientLeftCallback(NetworkConnectionToClient conn, ClientLeftMessage message)
        {
            int guid = conn.connectionId;
            //Find and remove all RacePlayers associated with players from this client
            //TODO: Find some way to still have the player in the race, although disabled - so that players leaving while finished don't just disappear
            foreach (RacePlayer racePlayer in players.ToList())
            {
                if (racePlayer.AssociatedMatchPlayer != null && racePlayer.AssociatedMatchPlayer.ConnectionId == guid)
                {
                    racePlayer.Destroy();
                    players.Remove(racePlayer);
                }
            }
        }

        public void DoneRacingCallback(DoneRacingMessage message)
        {
            var ctrlType = message.CtrlType;
            var raceTimer = message.RaceTime;
            var vl = message.Disqualified;
            RacePlayer rp = players.FirstOrDefault(a => a.AssociatedMatchPlayer != null
            && a.AssociatedMatchPlayer.ConnectionId == message.ConnectionID
            && a.AssociatedMatchPlayer.CtrlType == ctrlType);

            DoneRacingInner(rp, raceTimer, vl);
        }       
    }
}