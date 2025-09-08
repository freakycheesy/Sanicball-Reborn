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
    public enum RaceState
    {
        None,
        Waiting,
        Countdown,
        Racing,
        Finished
    }

    public partial class RaceManager : MonoBehaviour
    {
        public void Countdown_OnCountdownFinished(object sender, System.EventArgs e)
        {
            CurrentState = RaceState.Racing;
        }

        [Server]
        public void Init(MatchSettings settings, MatchManager matchManager, bool raceIsInProgress)
        {
            this.settings = settings;
            this.matchManager = matchManager;

            if (raceIsInProgress)
            {
                Debug.Log("Starting race in progress");
                joinedWhileRaceInProgress = true;
                CreateBallObjects();
            }
        }
        
        public void CreateBallObjects()
        {
            int nextBallPosition = 0;
            RaceBallSpawner ballSpawner = StageReferences.Active.spawnPoint;

            //Enable lap records if there is only one local player.
            bool enableLapRecords = matchManager.Players.Count(a => MatchManager.IsLocalId(a.ConnectionId)) == 1;

            //Create all player balls
            for (int i = 0; i < matchManager.Players.Count; i++)
            {
                var matchPlayer = matchManager.Players[i];

                bool local = MatchManager.IsLocalId(matchPlayer.ConnectionId);

                //Create ball
                string name = matchManager.Clients.FirstOrDefault(a => MatchManager.IsLocalId(a.ConnectionId)).Name;
                NetworkServer.connections.TryGetValue(matchPlayer.ConnectionId, out var conn);
                matchPlayer.BallObject = ballSpawner.SpawnBall(
                    nextBallPosition,
                    BallType.Player,
                    local ? matchPlayer.CtrlType : ControlType.None,
                    matchPlayer.CharacterId,
                    name + " (" + GameInput.GetControlTypeName(matchPlayer.CtrlType) + ")", conn
                    );

                //Create race player
                var racePlayer = new RacePlayer(matchPlayer.BallObject, matchPlayer);
                players.Add(racePlayer);
                racePlayer.LapRecordsEnabled = enableLapRecords && local;
                racePlayer.FinishLinePassed += RacePlayer_FinishLinePassed;

                nextBallPosition++;
            }


                //Create all AI balls (In local play only)
                for (int i = 0; i < settings.AICount; i++)
                {
                //Create ball
                var aiBall = ballSpawner.SpawnBall(
                    nextBallPosition,
                    BallType.AI,
                    ControlType.None,
                    i,
                    "AI #" + i
                    , NetworkServer.localConnection
                        );
                    aiBall.CanMove = false;

                    //Create race player
                    var racePlayer = new RacePlayer(aiBall, null);
                    players.Add(racePlayer);
                    racePlayer.FinishLinePassed += RacePlayer_FinishLinePassed;

                    nextBallPosition++;
                }

            int nextLocalPlayerIndex = 0;
            foreach (RacePlayer p in players.Where(a => a.CtrlType != ControlType.None))
            {
                //Create player UI
                var playerUI = Instantiate(playerUIPrefab);
                playerUI.TargetManager = this;
                playerUI.TargetPlayer = p;

                //Connect UI to camera (when the camera has been instanced)
                int persistentIndex = nextLocalPlayerIndex;
                p.AssociatedMatchPlayer.BallObject.CameraCreated += (e) =>
                {
                    playerUI.TargetCamera = e.CameraCreated.AttachedCamera;
                    var splitter = e.CameraCreated.AttachedCamera.GetComponent<CameraSplitter>();
                    if (splitter)
                        splitter.SplitscreenIndex = persistentIndex;
                };

                nextLocalPlayerIndex++;
            }
        }

        public void RacePlayer_FinishLinePassed(object sender, System.EventArgs e)
        {
            //Every time a player passes the finish line, check if it's done
            var rp = (RacePlayer)sender;

            if (rp.FinishReport == null && rp.Lap > settings.Laps)
            {
                //Race finishing is handled differently depending on what type of racer this is

                if (rp.AssociatedMatchPlayer != null)
                {
                    if (MatchManager.IsLocalId(rp.AssociatedMatchPlayer.ConnectionId))
                    {
                        //For local player balls, send a DoneRacingMessage.
                        DoneRacingMessage message = new(rp.AssociatedMatchPlayer.ConnectionId, rp.AssociatedMatchPlayer.CtrlType, raceTimer, false);
                        NetworkClient.Send<DoneRacingMessage>(message);
                        //LobbyScript.Instance.DoneRacingRpc(rp.AssociatedMatchPlayer.ClientGuid, rp.AssociatedMatchPlayer.CtrlType, raceTimer, false);
                    }
                    //For remote player balls, do nothing.
                }
                else
                {
                    //For AI balls, call DoneRacingInner directly to bypass the messaging system.
                    DoneRacingInner(rp, raceTimer, false);
                }
            }
        }
        public void DoneRacingInner(RacePlayer rp, double raceTime, bool disqualified)
        {
            int pos = players.IndexOf(rp) + 1;
            if (disqualified) pos = RaceFinishReport.DISQUALIFIED_POS;
            rp.FinishRace(new RaceFinishReport(pos, System.TimeSpan.FromSeconds(raceTime)));

            //Display scoreboard when all players have finished
            //TODO: Make proper scoreboard and have it trigger when only local players have finished
            if (!players.Any(a => a.IsPlayer && !a.RaceFinished))
            {
                StageReferences.Active.endOfMatchHandler.Activate(this);
            }
        }

        #region Unity event functions
        public void Awake()
        {
            Instance = this;
        }
        public void Start()
        {
            if (joinedWhileRaceInProgress)
            {
                CurrentState = RaceState.Racing;

                var specView = Instantiate(spectatorViewPrefab);
                specView.TargetManager = this;
                specView.Target = players[0];
            }
            else
            {
                CurrentState = RaceState.Waiting;
            }
            RegisterMessages();
            //LobbyScript.Instance.StartRaceRpc();
            foreach (var p in MatchManager.Instance.Players)
            {
                if (NetworkServer.localConnection.connectionId == p.ConnectionId)
                {
                    NetworkClient.Send<ReadyUpRaceMessage>(new(p.ConnectionId, p.CtrlType));
                    //LobbyScript.Instance.ReadyUpRaceRpc(p.CtrlType);
                }
            }
        }

        private void RegisterMessages()
        {
            NetworkServer.ReplaceHandler<DoneRacingMessage>(DoneRacingCallback);
            NetworkServer.ReplaceHandler<ClientLeftMessage>(ClientLeftCallback);
            NetworkServer.ReplaceHandler<ReadyUpRaceMessage>(ReadyUpRace);
        }

        public void Update()
        {
            //Increment the race timer if it's been started
            if (raceTimerOn)
            {
                raceTimer += Time.deltaTime;
                foreach (var p in players) p.UpdateTimer(Time.deltaTime);
            }
            //Order player list by position
            players = players.OrderByDescending(a => a.CalculateRaceProgress()).ToList();
            for (int i = 0; i < players.Count; i++)
                players[i].Position = i + 1;
        }

        public void OnDestroy()
        {
            //ALL listeners created in Init() should be removed from the messenger here
            //Otherwise the race manager won't get destroyed properly

            //Call the Destroy method on all players to properly dispose them
            foreach (RacePlayer p in players)
            {
                p.Destroy();
            }
        }


        public struct ReadyUpRaceMessage : NetworkMessage {
            public ControlType ctrlType;

            public ReadyUpRaceMessage(int myGuid, ControlType ctrlType)
            {
                this.ctrlType = ctrlType;
            }

        }

        [Server]
        public void ReadyUpRace(NetworkConnectionToClient conn, ReadyUpRaceMessage message)
        {
            var players = MatchManager.Instance.Players.FindAll(x => x.ConnectionId == conn.connectionId);
            foreach (var player in players)
            {
                player.ReadyToRace = true;
            }
            var canStartRace = true;
            foreach (var player in MatchManager.Instance.Players)
            {
                if (!player.ReadyToRace) canStartRace = false;
            }
            if (canStartRace) Invoke(nameof(StartRaceCallback), 8);
        }

        #endregion Unity event functions
    }
}