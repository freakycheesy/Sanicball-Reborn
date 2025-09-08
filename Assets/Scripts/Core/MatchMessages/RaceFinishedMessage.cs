using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceFinishedMessage : NetworkMessage
    {
        public int ConnectionID ;
        public ControlType CtrlType ;
        public float RaceTime ;
        public int RacePosition ;

        public RaceFinishedMessage(int clientGuid, ControlType ctrlType, float raceTime, int racePosition)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            RacePosition = racePosition;
        }
    }
}