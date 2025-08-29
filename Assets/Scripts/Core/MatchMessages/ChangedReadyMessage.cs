using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct ChangedReadyMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public bool Ready { get; private set; }

        public ChangedReadyMessage(NetworkConnection clientGuid, ControlType ctrlType, bool ready)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            Ready = ready;
        }
    }
}