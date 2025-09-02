using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }
        public string ClientName { get; private set; }

        public ClientJoinedMessage(int clientGuid, string clientName)
        {
            ConnectionID = clientGuid;
            ClientName = clientName;
        }
    }
}