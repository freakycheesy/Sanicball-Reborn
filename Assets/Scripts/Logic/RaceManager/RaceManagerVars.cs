using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Sanicball.Data;
using Sanicball.Gameplay;
using Sanicball.UI;
using SanicballCore;
using SanicballCore.MatchMessages;
using UnityEngine;

namespace Sanicball.Logic
{
    public partial class RaceManager : MonoBehaviour
    {
        //Prefabs
        [SerializeField]
        public WaitingCamera waitingCamPrefab = null;
        [SerializeField]
        public WaitingUI waitingUIPrefab = null;
        [SerializeField]
        public RaceCountdown raceCountdownPrefab = null;
        [SerializeField]
        public PlayerUI playerUIPrefab = null;
        [SerializeField]
        public RaceUI raceUIPrefab = null;
        [SerializeField]
        public SpectatorView spectatorViewPrefab = null;

        //Race state
        public List<RacePlayer> players = new List<RacePlayer>();
        public RaceState currentState = RaceState.Waiting;

        //Fields set in Init()
        public MatchSettings settings;
        public MatchManager matchManager;

        //Misc
        public WaitingCamera activeWaitingCam;
        public WaitingUI activeWaitingUI;
        public double raceTimer = 0;
        public bool raceTimerOn = false;
        public RaceUI raceUI;
        public float countdownOffset;
        public bool joinedWhileRaceInProgress;

        //Properties
        public System.TimeSpan RaceTime { get { return System.TimeSpan.FromSeconds(raceTimer); } }
        public MatchSettings Settings { get { return settings; } }

        //PlayerCount gets number of players, indexer lets you retrieve them
        public int PlayerCount { get { return players.Count; } }
        public RacePlayer this[int playerIndex] { get { return players[playerIndex]; } }

        //Return players as read only collection (easier to query)
        public System.Collections.ObjectModel.ReadOnlyCollection<RacePlayer> Players { get { return new System.Collections.ObjectModel.ReadOnlyCollection<RacePlayer>(players); } }
        public static RaceManager Instance;
        public RaceState CurrentState
        {
            get
            {
                return currentState;
            }

            set
            {
                //Shut down old state
                switch (currentState)
                {
                    case RaceState.Waiting:
                        if (activeWaitingCam)
                            Destroy(activeWaitingCam.gameObject);
                        if (activeWaitingUI)
                            Destroy(activeWaitingUI.gameObject);
                        break;
                }
                //Start new state
                switch (value)
                {
                    case RaceState.Waiting:
                        activeWaitingCam = Instantiate(waitingCamPrefab);
                        activeWaitingUI = Instantiate(waitingUIPrefab);
                        activeWaitingUI.StageNameToShow = ActiveData.GetStageByBarcode(settings.StageBarcode).name;
                        if (joinedWhileRaceInProgress)
                        {
                            activeWaitingUI.InfoToShow = "Waiting for race to end.";
                        }
                        else
                        {
                            activeWaitingUI.InfoToShow = "Waiting for other players...";
                        }

                        break;

                    case RaceState.Countdown:
                        //Create countdown
                        var countdown = Instantiate(raceCountdownPrefab);
                        countdown.ApplyOffset(countdownOffset);
                        countdown.OnCountdownFinished += Countdown_OnCountdownFinished;

                        //Create race UI
                        raceUI = Instantiate(raceUIPrefab);
                        raceUI.TargetManager = this;

                        //Create all balls
                        CreateBallObjects();

                        //If there are no local players, create a spectator camera
                        var hasPlayers = matchManager.Players != null;
                        if (!matchManager.Players.Any(a => MatchManager.IsLocalId(a.ConnectionId)))
                        {
                            var specView = Instantiate(spectatorViewPrefab);
                            specView.TargetManager = this;
                            specView.Target = players[0];
                        }

                        break;

                    case RaceState.Racing:
                        raceTimerOn = true;
                        var music = MusicPlayer.Instance;
                        if (music)
                        {
                            music.Play();
                        }
                        foreach (var p in players)
                        {
                            p.StartRace();
                        }
                        break;
                }
                currentState = value;
            }
        }
    }
}