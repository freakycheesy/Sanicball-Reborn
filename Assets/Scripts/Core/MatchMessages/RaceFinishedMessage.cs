using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct RaceFinishedMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float RaceTime { get; private set; }
        public int RacePosition { get; private set; }

        public RaceFinishedMessage(NetworkConnection clientGuid, ControlType ctrlType, float raceTime, int racePosition)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            RaceTime = raceTime;
            RacePosition = racePosition;
        }
    }
}