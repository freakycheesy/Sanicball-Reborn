using System;
using System.Collections.ObjectModel;
using System.Linq;
using Mirror;
using Sanicball.UI;
using SanicballCore;
using UnityEngine;

namespace Sanicball.Logic
{
    /// <summary>
    /// Manages game state - scenes, players, all that jazz
    /// </summary>
    public partial class MatchManager : NetworkBehaviour
    {
        #region Exposed fields

        public static MatchManager Instance;

        //Prefabs
        [SerializeField]
        public PauseMenu pauseMenuPrefab;
        [SerializeField]
        public Chat chatPrefab;
        [SerializeField]
        public RaceManager raceManagerPrefab;
        [SerializeField]
        public UI.Popup disconnectedPopupPrefab;
        [SerializeField]
        public Marker markerPrefab = null;

        #endregion Exposed fields

        #region Match state

        //List of all clients in the match. Only serves a purpose in online play.
        //In local play, this list will always only contain the local client.
        public readonly SyncList<MatchClient> Clients = new SyncList<MatchClient>();

        //List of all players - players are seperate from clients because each client can have
        //up to 4 players playing in splitscreen.
        public readonly SyncList<MatchPlayer> Players = new SyncList<MatchPlayer>();

        //Lobby countdown timer stuff
        public bool lobbyTimerOn = false;
        public const float lobbyTimerMax = 3;
        public float lobbyTimer = lobbyTimerMax;

        //Auto start timer (Only used in online mode)
        public bool autoStartTimerOn = false;
        public float autoStartTimer = 0;

        #endregion Match state

        #region Scenes and scene initializing

        //Bools for scene initializing and related stuff
        [SyncVar] public bool inLobby = false; //Are we in the lobby or in a race?
        [SyncVar] public bool loadingLobby = false;
        [SyncVar] public bool loadingStage = false;
        public bool joiningRaceInProgress = false; //If true, RaceManager will be created as if a race was already in progress.
        public bool showSettingsOnLobbyLoad = false; //If true, the match settings window will pop up when the lobby scene is entered.

        #endregion Scenes and scene initializing

        //Match messenger used to send and receive state changes.
        //This will be either a LocalMatchMessenger or OnlineMatchMessenger, but each are used the same way.

        public UI.Chat activeChat;

        //Timer used for syncing realtime stuff in online
        #region Properties

        /// <summary>
        /// Current settings for this match. On remote clients, this is only used for showing settings on the UI.
        /// </summary>
        [SerializeField, SyncVar] public MatchSettings CurrentSettings = MatchSettings.CreateDefault();

        public bool AutoStartTimerOn { get { return autoStartTimerOn; } }
        public float AutoStartTimer { get { return autoStartTimer; } }
        public bool InLobby { get { return inLobby; } }

        #endregion Properties

        #region State changing methods
        [TextArea(1, 50)]
        public string matchSettingJson;
        #endregion State changing methods
    }
}
