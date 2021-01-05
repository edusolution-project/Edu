using Lib.Net.Http.WebPush;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebPush.Interfaces
{
    public interface IPushSubscriptionStore
    {
        Task StoreSubscriptionAsync(PushSubscription subscription);

        Task DiscardSubscriptionAsync(string endpoint);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action);

        Task ForEachSubscriptionAsync(Action<PushSubscription> action, CancellationToken cancellationToken);
    }
}
