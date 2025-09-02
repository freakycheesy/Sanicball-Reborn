using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerJoinedMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public int InitialCharacter { get; private set; }

        public PlayerJoinedMessage(int clientGuid, ControlType ctrlType, int initialCharacter)
        {
            ConnectionID = clientGuid;
            CtrlType = ctrlType;
            InitialCharacter = initialCharacter;
        }
    }
}