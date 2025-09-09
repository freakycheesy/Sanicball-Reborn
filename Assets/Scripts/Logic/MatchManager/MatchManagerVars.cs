using System;
using System.Collections.Generic;
using Mirror;
using Sanicball.Data;
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

        //Match messenger used to send and receive state changes.
        //This will be either a LocalMatchMessenger or OnlineMatchMessenger, but each are used the same way.

        public UI.Chat activeChat;
        [SyncVar] public MatchState state = new();

        public MatchClient localClient = new();

        //Timer used for syncing realtime stuff in online
        #region Properties
        public MatchState State => state;
        public List<MatchClient> Clients => state.clients;
        public List<MatchPlayer> Players => state.players;
        public bool AutoStartTimerOn { get { return state.autoStartTimerOn; } }
        public float AutoStartTimer { get { return state.autoStartTimer; } }
        public bool InLobby { get { return state.inLobby; } }

        #endregion Properties

        #region State changing methods
        [TextArea(1, 50)]
        public string matchSettingJson;
        #endregion State changing methods
    }

    [Serializable]
    public struct MatchState
    {
        public MatchState(
            MatchSettings MatchSettings = new()
          )
        {
            clients = new();
            players = new();
            CurrentSettings = MatchSettings;
            lobbyTimerOn = false;
            lobbyTimer = lobbyTimerMax;
            autoStartTimerOn = false;
            autoStartTimer = 0;
            inLobby = false;
            loadingLobby = false;
            loadingStage = false;
            joiningRaceInProgress = false;
            showSettingsOnLobbyLoad = false;
        }
        #region Match state

        //List of all clients in the match. Only serves a purpose in online play.
        //In local play, this list will always only contain the local client.
        public List<MatchClient> clients;

        //List of all players - players are seperate from clients because each client can have
        //up to 4 players playing in splitscreen.
        public List<MatchPlayer> players;

        //Lobby countdown timer stuff
        public bool lobbyTimerOn;
        public const float lobbyTimerMax = 3;
        public float lobbyTimer;

        //Auto start timer (Only used in online mode)
        public bool autoStartTimerOn;
        public float autoStartTimer;

        #endregion Match state

        #region Scenes and scene initializing

        //Bools for scene initializing and related stuff
        public bool inLobby; //Are we in the lobby or in a race?
        public bool loadingLobby;
        public bool loadingStage;
        public bool joiningRaceInProgress; //If true, RaceManager will be created as if a race was already in progress.
        public bool showSettingsOnLobbyLoad; //If true, the match settings window will pop up when the lobby scene is entered.
        #endregion Scenes and scene initializing
        public MatchSettings CurrentSettings;
    }
}
