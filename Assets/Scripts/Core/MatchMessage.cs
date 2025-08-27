using FishNet.Broadcast;

namespace SanicballCore
{
    public delegate void MatchMessageHandler<T>(T message, float travelTime);
}