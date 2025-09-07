using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerJoinedMessage : NetworkMessage
    {
        public ControlType CtrlType { get; private set; }
        public int InitialCharacter { get; private set; }

        public PlayerJoinedMessage(ControlType ctrlType, int initialCharacter)
        {
            CtrlType = ctrlType;
            InitialCharacter = initialCharacter;
        }
    }
}