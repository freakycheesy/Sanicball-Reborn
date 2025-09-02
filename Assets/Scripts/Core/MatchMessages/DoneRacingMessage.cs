using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct DoneRacingMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public double RaceTime { get; private set; }
        public bool Disqualified { get; private set; }

        public DoneRacingMessage(int clientGuid, ControlType ctrlType, double raceTime, bool disqualified)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            Disqualified = disqualified;
        }
    }
}