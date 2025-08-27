using System;
using FishNet.Broadcast;
using Newtonsoft.Json;
using Unity.Mathematics;
using UnityEngine;

namespace SanicballCore
{
    [JsonObject, Serializable]
    public struct ZaLobbyInfo : IBroadcast
    {
        [Header("Realtime Info")]
        public int Players;
        [Header("Static Info")]
        public int MaxPlayers;
        public string Name;
        public string IP;
        public ushort Port;
        public bool InRace { get; set; }
        public ZaLobbyInfo(string IP = "127.0.0.1", ushort Port = 7778, bool InRace = false, int MaxPlayers = 8, string Name = "My Lobby")
        {
            this.Players = 0;
            this.MaxPlayers = MaxPlayers;
            this.Name = Name;
            this.IP = IP;
            this.Port = Port;
            this.InRace = InRace;
        }
    }
}