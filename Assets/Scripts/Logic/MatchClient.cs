using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Sanicball.Logic
{
    [Serializable]
    public struct MatchClient
    {
        public int ConnectionId { get; private set; }
        public string Name { get; private set; }

        public MatchClient(int guid,string name)
        {
            ConnectionId = guid;
            Name = name;
        }
    }
}