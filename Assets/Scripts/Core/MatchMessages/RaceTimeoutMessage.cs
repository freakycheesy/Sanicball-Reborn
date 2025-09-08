using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceTimeoutMessage : NetworkMessage
    {
        public int ConnectionID ;
        public ControlType CtrlType ;
        public float Time ;

        public RaceTimeoutMessage(int clientGuid, ControlType ctrlType, float time)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            Time = time;
        }
    }
}