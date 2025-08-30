using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SanicballCore;
using UnityEngine;
using Sanicball.Data;
using FishNet.Connection;

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
        private NetworkConnection clientGuid;
        private ControlType ctrlType;

        public MatchPlayer(NetworkConnection clientGuid, ControlType ctrlType, int initialCharacterId)
        {
            this.clientGuid = clientGuid;
            this.ctrlType = ctrlType;
            CharacterId = initialCharacterId;
        }

        public NetworkConnection ClientGuid { get { return clientGuid; } }
        public ControlType CtrlType { get { return ctrlType; } }
        public int CharacterId { get; set; }
        public Gameplay.Ball BallObject { get; set; }
        public bool ReadyToRace { get; set; }

		public static MatchPlayer GetByBall(Gameplay.Ball referenceBall) {
			object[] gameobjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			foreach (object obj in gameobjects) {
				//Debug.Log("Testing if Object "+((GameObject) obj).name+" has MatchPlayer.");
				MatchPlayer player = ((GameObject) obj).GetComponent<MatchPlayer>();
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