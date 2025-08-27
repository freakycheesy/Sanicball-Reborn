using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct AutoStartTimerMessage : IBroadcast
    {
        public bool Enabled { get; private set; }

        public AutoStartTimerMessage(bool enabled)
        {
            Enabled = enabled;
        }
    }
}