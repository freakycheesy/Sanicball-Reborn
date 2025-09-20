using System;
using Mirror;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public partial class MatchManager : NetworkBehaviour
    {
        public static EventHandler<MatchPlayerEventArgs> MatchPlayerAdded;
        public static EventHandler<MatchPlayerEventArgs> MatchPlayerRemoved;
        public static EventHandler MatchManagerUpdated;
        public static EventHandler<float> MatchManagerSpawned;
        public static EventHandler<MatchSettings> MatchSettingsChanged;
    }
}
