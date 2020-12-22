using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using WP = Lib.Net.Http.WebPush;
using WebPush.Interfaces;

namespace WebPush.Services.Sqlite
{
    internal class SqlitePushSubscriptionStore : IPushSubscriptionStore
    {
        private readonly PushSubscriptionContext _context;

        public SqlitePushSubscriptionStore(PushSubscriptionContext context)
        {
            _context = context;
        }

        public Task StoreSubscriptionAsync(WP.PushSubscription subscription)
        {
            return _context.CreateQuery().InsertOneAsync(new PushSubscription(subscription));
        }

        public async Task DiscardSubscriptionAsync(string endpoint)
        {
            var data = _context.CreateQuery()?.Find(o => o.Endpoint == endpoint)?.SingleOrDefault();
            if(data != null)
            {
               await _context.CreateQuery().DeleteOneAsync(o => o.Endpoint == data.Endpoint);
            }
        }

        public Task ForEachSubscriptionAsync(Action<WP.PushSubscription> action)
        {
            return ForEachSubscriptionAsync(action, CancellationToken.None);
        }

        public Task ForEachSubscriptionAsync(Action<WP.PushSubscription> action, CancellationToken cancellationToken)
        {
            return _context.CreateQuery().Find(o => true).ForEachAsync(action, cancellationToken);
        }
    }
}
