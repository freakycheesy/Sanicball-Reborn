using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct DoneRacingMessage : NetworkMessage
    {
        public int ConnectionID ;
        public ControlType CtrlType ;
        public double RaceTime ;
        public bool Disqualified ;

        public DoneRacingMessage(int clientGuid, ControlType ctrlType, double raceTime, bool disqualified)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            Disqualified = disqualified;
        }
    }
}