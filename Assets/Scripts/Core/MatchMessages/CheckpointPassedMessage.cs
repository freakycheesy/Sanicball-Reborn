using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CheckpointPassedMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float LapTime { get; private set; }

        public CheckpointPassedMessage(int clientGuid, ControlType ctrlType, float lapTime)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            LapTime = lapTime;
        }
    }
}