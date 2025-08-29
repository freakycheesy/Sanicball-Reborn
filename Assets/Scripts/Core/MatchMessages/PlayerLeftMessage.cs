using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct PlayerLeftMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }

        public PlayerLeftMessage(NetworkConnection clientGuid, ControlType ctrlType)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
        }
    }
}