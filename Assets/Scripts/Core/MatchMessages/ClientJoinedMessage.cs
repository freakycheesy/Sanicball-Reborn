using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }
        public string ClientName { get; private set; }

        public ClientJoinedMessage(NetworkConnection clientGuid, string clientName)
        {
            ClientGuid = clientGuid;
            ClientName = clientName;
        }
    }
}