using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct CheckpointPassedMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float LapTime { get; private set; }

        public CheckpointPassedMessage(System.Guid clientGuid, ControlType ctrlType, float lapTime)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            LapTime = lapTime;
        }
    }
}