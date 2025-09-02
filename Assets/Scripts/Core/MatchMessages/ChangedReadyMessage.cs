using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ChangedReadyMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public ControlType CtrlType { get; private set; }
        public bool Ready { get; private set; }

        public ChangedReadyMessage(int connectionID, ControlType ctrlType, bool ready)
        {
            ConnectionID = connectionID;
            CtrlType = ctrlType;
            Ready = ready;
        }
    }
}