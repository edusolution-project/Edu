using System;
using System.Threading;
using System.Threading.Tasks;
using WebPush.Models;
using WebPush.Services.Sqlite;

namespace WebPush.Interfaces
{
    public interface IPushSubscriptionStore
    {
        Task StoreSubscriptionAsync(PushSubscriptionEntity subscription);

        Task DiscardSubscriptionAsync(string endpoint);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken);
    }
}
