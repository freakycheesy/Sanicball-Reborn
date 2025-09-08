using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CharacterChangedMessage : NetworkMessage
    {
        public ControlType CtrlType;
        public int NewCharacter;

        public CharacterChangedMessage(ControlType ctrlType, int newCharacter)
        {
            CtrlType = ctrlType;
            NewCharacter = newCharacter;
        }
    }
}