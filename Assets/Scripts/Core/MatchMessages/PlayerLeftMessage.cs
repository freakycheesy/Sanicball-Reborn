using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerLeftMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }

        public PlayerLeftMessage(int clientGuid, ControlType ctrlType)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
        }
    }
}