using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct PlayerZappedMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }

        public PlayerZappedMessage(System.Guid clientGuid, ControlType ctrlType)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
        }
    }
}