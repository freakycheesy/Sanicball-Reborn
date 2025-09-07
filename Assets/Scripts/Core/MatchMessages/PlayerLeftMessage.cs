using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerLeftMessage : NetworkMessage
    {
        public ControlType CtrlType { get; private set; }

        public PlayerLeftMessage(ControlType ctrlType)
        {
            CtrlType = ctrlType;
        }
    }
}