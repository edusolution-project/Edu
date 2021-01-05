using Lib.Net.Http.WebPush;
using System.Threading;
using System.Threading.Tasks;

namespace WebPush.Interfaces
{
    public interface IPushNotificationsQueue
    {
        void Enqueue(PushMessage message);

        Task<PushMessage> DequeueAsync(CancellationToken cancellationToken);
    }
}
