using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : NetworkMessage
    {
        public string ClientName;

        public ClientJoinedMessage(string clientName)
        {
            ClientName = clientName;
        }
    }
}