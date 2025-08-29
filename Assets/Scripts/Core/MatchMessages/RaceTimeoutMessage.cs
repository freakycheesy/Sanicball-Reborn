using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct RaceTimeoutMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public ControlType CtrlType { get; private set; }
        public float Time { get; private set; }

        public RaceTimeoutMessage(NetworkConnection clientGuid, ControlType ctrlType, float time)
        {
            ClientGuid = clientGuid;
            CtrlType = ctrlType;
            Time = time;
        }
    }
}