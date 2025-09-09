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
        public string Name;

        public MatchClient(int guid,string name)
        {
            ConnectionId = guid;
            Name = name;
        }
    }
}