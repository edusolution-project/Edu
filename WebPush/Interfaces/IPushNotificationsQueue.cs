using Lib.Net.Http.WebPush;
using System.Threading;
using System.Threading.Tasks;
using WebPush.Models;

namespace WebPush.Interfaces
{
    public interface IPushNotificationsQueue
    {
        void Enqueue(PushMessageEntity message);

        Task<PushMessageEntity> DequeueAsync(CancellationToken cancellationToken);
    }
}
