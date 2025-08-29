using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct CheckpointPassedMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float LapTime { get; private set; }

        public CheckpointPassedMessage(NetworkConnection clientGuid, ControlType ctrlType, float lapTime)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            LapTime = lapTime;
        }
    }
}