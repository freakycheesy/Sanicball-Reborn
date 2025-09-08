using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct SettingsChangedMessage : NetworkMessage
    {
        public MatchSettings NewMatchSettings ;

        public SettingsChangedMessage(MatchSettings newMatchSettings)
        {
            NewMatchSettings = newMatchSettings;
        }
    }
}