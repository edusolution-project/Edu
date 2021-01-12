using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Lib.Net.Http.WebPush;
using WebPush.Interfaces;
using WebPush.Models;

namespace WebPush.Services
{
    internal class PushNotificationsQueue : IPushNotificationsQueue
    {
        private readonly ConcurrentQueue<PushMessageEntity> _messages = new ConcurrentQueue<PushMessageEntity>();
        private readonly SemaphoreSlim _messageEnqueuedSignal = new SemaphoreSlim(0);

        public void Enqueue(PushMessageEntity message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            _messages.Enqueue(message);

            _messageEnqueuedSignal.Release();
        }

        public async Task<PushMessageEntity> DequeueAsync(CancellationToken cancellationToken)
        {
            await _messageEnqueuedSignal.WaitAsync(cancellationToken);

            _messages.TryDequeue(out PushMessageEntity message);

            return message;
        }
    }
}
