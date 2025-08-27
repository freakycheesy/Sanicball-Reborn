using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }
        public string ClientName { get; private set; }

        public ClientJoinedMessage(System.Guid clientGuid, string clientName)
        {
            ClientGuid = clientGuid;
            ClientName = clientName;
        }
    }
}