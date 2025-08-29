using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct CharacterChangedMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public int NewCharacter { get; private set; }

        public CharacterChangedMessage(NetworkConnection clientGuid, ControlType ctrlType, int newCharacter)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            NewCharacter = newCharacter;
        }
    }
}