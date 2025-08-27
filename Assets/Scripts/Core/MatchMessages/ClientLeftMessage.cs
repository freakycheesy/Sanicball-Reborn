using FishNet.Broadcast;

namespace SanicballCore.MatchMessages
{
    public struct ClientLeftMessage : IBroadcast
    {
        public System.Guid ClientGuid { get; private set; }

        public ClientLeftMessage(System.Guid clientGuid)
        {
            ClientGuid = clientGuid;
        }
    }
}