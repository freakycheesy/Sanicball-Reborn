using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ChangedReadyMessage : NetworkMessage
    {
        public ControlType CtrlType;
        public bool Ready;

        public ChangedReadyMessage(ControlType ctrlType, bool ready)
        {
            CtrlType = ctrlType;
            Ready = ready;
        }
    }
}