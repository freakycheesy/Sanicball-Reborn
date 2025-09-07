using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : NetworkMessage
    {
        public string ClientName { get; private set; }

        public ClientJoinedMessage(string clientName)
        {
            ClientName = clientName;
        }
    }
}