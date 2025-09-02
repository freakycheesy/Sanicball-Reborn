using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct SettingsChangedMessage : NetworkMessage
    {
        public MatchSettings NewMatchSettings { get; private set; }

        public SettingsChangedMessage(MatchSettings newMatchSettings)
        {
            NewMatchSettings = newMatchSettings;
        }
    }
}