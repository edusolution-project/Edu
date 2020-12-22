using System.Threading.Tasks;
using System.Collections.Generic;
using Lib.Net.Http.WebPush;

namespace WebPush.Services.Cosmos
{
    internal interface IPushSubscriptionCosmosDbClient
    {
        Task EnsureCreatedAsync();

        Task AddAsync(PushSubscription subscription);

        Task RemoveAsync(string endpoint);

        Task<IEnumerable<PushSubscription>> GetAllAsync();
    }
}
