using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct PlayerJoinedMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public int InitialCharacter { get; private set; }

        public PlayerJoinedMessage(NetworkConnection clientGuid, ControlType ctrlType, int initialCharacter)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            InitialCharacter = initialCharacter;
        }
    }
}