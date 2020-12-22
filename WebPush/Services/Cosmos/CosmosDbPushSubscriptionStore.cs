using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lib.Net.Http.WebPush;
using WebPush.Interfaces;

namespace WebPush.Services.Cosmos
{
    internal class CosmosDbPushSubscriptionStore : IPushSubscriptionStore
    {
        private readonly IPushSubscriptionCosmosDbClient _client;

        public CosmosDbPushSubscriptionStore(IPushSubscriptionCosmosDbClient client)
        {
            _client = client;
        }

        public Task StoreSubscriptionAsync(PushSubscription subscription)
        {
            return _client.AddAsync(subscription);
        }

        public Task DiscardSubscriptionAsync(string endpoint)
        {
            return _client.RemoveAsync(endpoint);
        }

        public Task ForEachSubscriptionAsync(Action<PushSubscription> action)
        {
            return ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public async Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken)
        {
            var data = await _client.GetAllAsync();
            for(int i = 0; data != null && i< data.Count(); i++)
            {
                PushSubscription subscription = data.ElementAt(i);
                action(subscription);
            }
        }
    }
}
