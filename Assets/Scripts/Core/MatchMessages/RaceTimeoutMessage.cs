using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceTimeoutMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float Time { get; private set; }

        public RaceTimeoutMessage(int clientGuid, ControlType ctrlType, float time)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            Time = time;
        }
    }
}