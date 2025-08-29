using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FishNet.Connection;
using UnityEngine;

namespace Sanicball.Logic
{
    [Serializable]
    public class MatchClient
    {
        public NetworkConnection Guid { get; private set; }
        public string Name { get; private set; }

        public MatchClient(NetworkConnection guid,string name)
        {
            Guid = guid;
            Name = name;
        }
    }
}