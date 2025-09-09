using Mirror;
using Sanicball.Logic;

namespace SanicballCore.MatchMessages
{
    public struct ClientJoinedMessage : NetworkMessage
    {
        public MatchClient Client;

        public ClientJoinedMessage(MatchClient Client)
        {
            this.Client = Client;
        }
    }
}