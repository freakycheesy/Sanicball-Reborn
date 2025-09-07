using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ChangedReadyMessage : NetworkMessage
    {
        public ControlType CtrlType { get; private set; }
        public bool Ready { get; private set; }

        public ChangedReadyMessage(ControlType ctrlType, bool ready)
        {
            CtrlType = ctrlType;
            Ready = ready;
        }
    }
}