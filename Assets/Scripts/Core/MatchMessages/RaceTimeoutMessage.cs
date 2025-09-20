using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct RaceTimeoutMessage : NetworkMessage
    {
        public ControlType CtrlType ;
        public float Time ;

        public RaceTimeoutMessage(ControlType ctrlType, float time)
        {
            CtrlType = ctrlType;
            Time = time;
        }
    }
}