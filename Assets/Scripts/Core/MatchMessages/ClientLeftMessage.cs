using FishNet.Broadcast;
using FishNet.Connection;

namespace SanicballCore.MatchMessages
{
    public struct ClientLeftMessage : IBroadcast
    {
        public NetworkConnection ClientGuid { get; private set; }

        public ClientLeftMessage(NetworkConnection clientGuid)
        {
            ClientGuid = clientGuid;
        }
    }
}