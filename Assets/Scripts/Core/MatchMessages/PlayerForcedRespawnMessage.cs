using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct PlayerForcedRespawnMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }

        public PlayerForcedRespawnMessage(System.Guid clientGuid, ControlType ctrlType)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
        }
    }
}