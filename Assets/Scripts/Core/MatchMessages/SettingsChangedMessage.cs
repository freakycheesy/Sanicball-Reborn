using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct SettingsChangedMessage : IBroadcast
    {
        public MatchSettings NewMatchSettings { get; private set; }

        public SettingsChangedMessage(MatchSettings newMatchSettings)
        {
            NewMatchSettings = newMatchSettings;
        }
    }
}