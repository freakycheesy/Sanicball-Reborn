using System;
using Mirror;
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
        public static Action<MatchManager, float> MatchManagerSpawned;
        public static EventHandler MatchSettingsChanged;
    }
}
