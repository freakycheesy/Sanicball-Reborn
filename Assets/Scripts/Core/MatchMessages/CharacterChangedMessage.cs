using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct CharacterChangedMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public int NewCharacter { get; private set; }

        public CharacterChangedMessage(System.Guid clientGuid, ControlType ctrlType, int newCharacter)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            NewCharacter = newCharacter;
        }
    }
}