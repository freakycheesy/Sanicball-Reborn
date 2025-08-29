using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct DoneRacingMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public double RaceTime { get; private set; }
        public bool Disqualified { get; private set; }

        public DoneRacingMessage(NetworkConnection clientGuid, ControlType ctrlType, double raceTime, bool disqualified)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            Disqualified = disqualified;
        }
    }
}