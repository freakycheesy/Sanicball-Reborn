using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct AutoStartTimerMessage : NetworkMessage
    {
        public bool Enabled;

        public AutoStartTimerMessage(bool enabled)
        {
            Enabled = enabled;
        }
    }
}