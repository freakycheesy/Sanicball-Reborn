using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceFinishedMessage : NetworkMessage
    {
        public ControlType CtrlType ;
        public float RaceTime ;
        public int RacePosition ;

        public RaceFinishedMessage(ControlType ctrlType, float raceTime, int racePosition)
        {
            CtrlType = ctrlType;
            RaceTime = raceTime;
            RacePosition = racePosition;
        }
    }
}