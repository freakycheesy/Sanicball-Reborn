using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Sanicball.Logic
{
    [System.Serializable]
    public struct MatchClient
    {
        public int ConnectionId;
        public NetworkConnectionToClient GetConnection()
        {
            NetworkServer.connections.TryGetValue(ConnectionId, out var value);
            return value;
        }
        public string Name;

        public MatchClient(NetworkConnectionToClient connection, string name)
        {
            ConnectionId = connection.connectionId;
            Name = name;
        }
    }
}