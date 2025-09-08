using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct PlayerJoinedMessage : NetworkMessage
    {
        public ControlType CtrlType ;
        public int InitialCharacter ;

        public PlayerJoinedMessage(ControlType ctrlType, int initialCharacter)
        {
            CtrlType = ctrlType;
            InitialCharacter = initialCharacter;
        }
    }
}