using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct CharacterChangedMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public int NewCharacter { get; private set; }

        public CharacterChangedMessage(int connectionID, ControlType ctrlType, int newCharacter)
        {
            ConnectionID = connectionID;
            CtrlType = ctrlType;
            NewCharacter = newCharacter;
        }
    }
}