using Mirror;

namespace SanicballCore.MatchMessages
{
    public struct ClientLeftMessage : NetworkMessage
    {
        public int ConnectionID { get; private set; }

        public ClientLeftMessage(int clientGuid)
        {
            ConnectionID = clientGuid;
        }
    }
}