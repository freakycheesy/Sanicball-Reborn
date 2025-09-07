using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CharacterChangedMessage : NetworkMessage
    {
        public ControlType CtrlType { get; private set; }
        public int NewCharacter { get; private set; }

        public CharacterChangedMessage(ControlType ctrlType, int newCharacter)
        {
            CtrlType = ctrlType;
            NewCharacter = newCharacter;
        }
    }
}