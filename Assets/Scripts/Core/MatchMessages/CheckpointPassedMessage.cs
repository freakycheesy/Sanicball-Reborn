using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CheckpointPassedMessage : NetworkMessage
    {
        public ControlType CtrlType;
        public float LapTime;

        public CheckpointPassedMessage(ControlType ctrlType, float lapTime)
        {
            CtrlType = ctrlType;
            LapTime = lapTime;
        }
    }
}