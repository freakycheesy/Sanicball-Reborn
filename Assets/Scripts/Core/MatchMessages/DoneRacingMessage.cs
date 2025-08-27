using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct DoneRacingMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public double RaceTime { get; private set; }
        public bool Disqualified { get; private set; }

        public DoneRacingMessage(System.Guid clientGuid, ControlType ctrlType, double raceTime, bool disqualified)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            Disqualified = disqualified;
        }
    }
}