using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct DoneRacingMessage : NetworkMessage
    {
        public ControlType CtrlType ;
        public double RaceTime ;
        public bool Disqualified ;

        public DoneRacingMessage(ControlType ctrlType, double raceTime, bool disqualified)
        {
            CtrlType = ctrlType;
            RaceTime = raceTime;
            Disqualified = disqualified;
        }
    }
}