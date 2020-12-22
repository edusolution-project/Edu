using WebPush.Interfaces;

namespace WebPush.Services.Cosmos
{
    internal class CosmosDbPushSubscriptionStoreAccessor : IPushSubscriptionStoreAccessor
    {
        public IPushSubscriptionStore PushSubscriptionStore { get; private set; }

        public CosmosDbPushSubscriptionStoreAccessor(IPushSubscriptionStore pushSubscriptionStore)
        {
            PushSubscriptionStore = pushSubscriptionStore;
        }

        public void Dispose()
        { }
    }
}
