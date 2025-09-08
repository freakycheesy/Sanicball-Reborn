using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CheckpointPassedMessage : NetworkMessage
    {
        public int ConnectionID;
        public ControlType CtrlType;
        public float LapTime;

        public CheckpointPassedMessage(int clientGuid, ControlType ctrlType, float lapTime)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            LapTime = lapTime;
        }
    }
}