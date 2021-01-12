using Lib.Net.Http.WebPush;
using System.Threading;
using System.Threading.Tasks;
using WebPush.Models;

namespace WebPush.Interfaces
{
    public interface IPushNotificationService
    {
        string PublicKey { get; }

        Task SendNotificationAsync(PushSubscription subscription, PushMessage message);

        Task SendNotificationAsync(PushSubscription subscription, PushMessage message, CancellationToken cancellationToken);
    }
}
