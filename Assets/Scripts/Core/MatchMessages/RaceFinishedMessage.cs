using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceFinishedMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float RaceTime { get; private set; }
        public int RacePosition { get; private set; }

        public RaceFinishedMessage(int clientGuid, ControlType ctrlType, float raceTime, int racePosition)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            RacePosition = racePosition;
        }
    }
}