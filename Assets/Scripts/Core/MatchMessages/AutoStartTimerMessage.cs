using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct AutoStartTimerMessage : NetworkMessage
    {
        public bool Enabled { get; private set; }

        public AutoStartTimerMessage(bool enabled)
        {
            Enabled = enabled;
        }
    }
}