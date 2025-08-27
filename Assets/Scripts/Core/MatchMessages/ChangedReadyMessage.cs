using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct ChangedReadyMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public bool Ready { get; private set; }

        public ChangedReadyMessage(System.Guid clientGuid, ControlType ctrlType, bool ready)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            Ready = ready;
        }
    }
}