using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using Sanicball.Data;
using UnityEngine;

namespace Sanicball.Logic
{
    public class MatchClient : NetworkBehaviour
    {
        public static List<MatchClient> Clients = new();
        [SyncVar] public int ConnectionId;
        [SyncVar] public string Nickname;
        public NetworkConnectionToClient GetConnection()
        {
            NetworkServer.connections.TryGetValue(ConnectionId, out var value);
            return value;
        }
        public static MatchClient LocalClient { get; private set; }
        public static Action<MatchClient> OnConnectedEvent;
        public static Action<MatchClient> OnDisconnectedEvent;
        void Start()
        {
            Debug.Log("New client " + Nickname + "(GENERAL)");
            Clients.Add(this);
            syncDirection = SyncDirection.ServerToClient;
            OnConnectedEvent?.Invoke(this);
            DontDestroyOnLoad(gameObject);
        }

        void OnDestroy()
        {
            Clients.Remove(this);
            OnDisconnectedEvent?.Invoke(this);
        }

        protected override void OnValidate()
        {
            syncDirection = SyncDirection.ServerToClient;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            LocalClient = this;
            SendInfoToServer(ActiveData.Instance.GameSettings.nickname);
        }

        [Command]
        private void SendInfoToServer(string nickname, NetworkConnectionToClient sender = null)
        {
            Debug.Log("New client " + Nickname + "(SERVER)");
            Nickname = nickname;
            ConnectionId = sender.connectionId;
        }
    }
}