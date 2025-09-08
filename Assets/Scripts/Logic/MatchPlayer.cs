using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SanicballCore;
using UnityEngine;
using Sanicball.Data;
using Mirror;
using Sanicball.Gameplay;

namespace Sanicball.Logic
{
    public class MatchPlayerEventArgs : EventArgs
    {
        public MatchPlayer Player { get; private set; }
        public bool IsLocal { get; private set; }

        public MatchPlayerEventArgs(MatchPlayer player, bool isLocal)
        {
            Player = player;
            IsLocal = isLocal;
        }
    }
    [Serializable]
    public class MatchPlayer
    {
        private int connectionId;
        [SerializeField]
        private ControlType ctrlType;

        public MatchPlayer(int clientGuid, ControlType ctrlType, int initialCharacterId, Ball BallObject = null, bool ReadyToRace = false)
        {
            this.connectionId = clientGuid;
            this.ctrlType = ctrlType;
            CharacterId = initialCharacterId;
            this.BallObject = BallObject;
            this.ReadyToRace = ReadyToRace;
        }

        public MatchPlayer()
        {
            connectionId = 0;
            ctrlType = ControlType.None;
            CharacterId = 0;
            BallObject = null;
            ReadyToRace = false;
        }

        public int ConnectionId { get { return connectionId; } }
        public ControlType CtrlType { get { return ctrlType; } }
        public int CharacterId { get; set; }
        public Ball BallObject { get; set; }
        public bool ReadyToRace { get; set; }

		public static MatchPlayer GetByBall(Gameplay.Ball referenceBall) {
			var gameobjects = Resources.FindObjectsOfTypeAll<Ball>();
			foreach (var obj in gameobjects) {
                //Debug.Log("Testing if Object "+((GameObject) obj).name+" has MatchPlayer.");
                MatchPlayer player = obj.GetComponent<MatchPlayer>();
				if(player != null) {
					//Debug.Log("Testing MatchPlayer with the referenceBall");
					if(player.BallObject == referenceBall){
						//Debug.Log("MatchPlayer matching referenceBall found. returning value");
						return player;
					}
				}
			}
			return null;
		}
    }
}