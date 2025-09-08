using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerLeftMessage : NetworkMessage
    {
        public ControlType CtrlType ;

        public PlayerLeftMessage(ControlType ctrlType)
        {
            CtrlType = ctrlType;
        }
    }
}