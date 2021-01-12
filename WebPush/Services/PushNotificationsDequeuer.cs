using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Lib.Net.Http.WebPush;
using WebPush.Interfaces;
using WebPush.Services.Sqlite;
using PushSubscription = WebPush.Services.Sqlite.PushSubscription;
using WebPush.Models;

namespace WebPush.Services
{
    internal class PushNotificationsDequeuer : IHostedService
    {
        private readonly IPushSubscriptionStoreAccessorProvider _subscriptionStoreAccessorProvider;
        private readonly IPushNotificationsQueue _messagesQueue;
        private readonly IPushNotificationService _notificationService;
        private readonly CancellationTokenSource _stopTokenSource = new CancellationTokenSource();

        private Task _dequeueMessagesTask;

        public PushNotificationsDequeuer(IPushNotificationsQueue messagesQueue, IPushSubscriptionStoreAccessorProvider subscriptionStoreAccessorProvider, IPushNotificationService notificationService)
        {
            _subscriptionStoreAccessorProvider = subscriptionStoreAccessorProvider;
            _messagesQueue = messagesQueue;
            _notificationService = notificationService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _dequeueMessagesTask = Task.Run(DequeueMessagesAsync);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _stopTokenSource.Cancel();

            return Task.WhenAny(_dequeueMessagesTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task DequeueMessagesAsync()
        {
            while (!_stopTokenSource.IsCancellationRequested)
            {
                PushMessageEntity message = await _messagesQueue.DequeueAsync(_stopTokenSource.Token);

                if (!_stopTokenSource.IsCancellationRequested)
                {
                    using (IPushSubscriptionStoreAccessor subscriptionStoreAccessor = _subscriptionStoreAccessorProvider.GetPushSubscriptionStoreAccessor())
                    {
                        await subscriptionStoreAccessor.PushSubscriptionStore.ForEachSubscriptionAsync((PushSubscription subscription) =>
                        {
                            // send all
                            if (string.IsNullOrEmpty(message.UserId))
                            {
                                subscription.Keys.Remove("UserId");
                                Lib.Net.Http.WebPush.PushSubscription pushSubscription = new Lib.Net.Http.WebPush.PushSubscription()
                                {
                                    Endpoint = subscription.Endpoint,
                                    Keys = subscription.Keys
                                };
                                // Fire-and-forget 
                                _notificationService.SendNotificationAsync(pushSubscription, message, _stopTokenSource.Token);
                            }
                            // send for user
                            else
                            {
                                if(subscription.UserId == message.UserId)
                                {
                                    subscription.Keys.Remove("UserId");
                                    Lib.Net.Http.WebPush.PushSubscription pushSubscription = new Lib.Net.Http.WebPush.PushSubscription()
                                    {
                                        Endpoint = subscription.Endpoint,
                                        Keys = subscription.Keys
                                    };
                                    _notificationService.SendNotificationAsync(pushSubscription, message, _stopTokenSource.Token);
                                }
                            }
                            
                        }, _stopTokenSource.Token);
                    }

                }
            }

        }
    }
}
