using Microsoft.Extensions.DependencyInjection;
using WebPush.Interfaces;

namespace WebPush.Services.Cosmos
{
    public static class CosmosDbServiceCollectionExtensions
    {
        public static IServiceCollection AddCosmosDbPushSubscriptionStore(this IServiceCollection services)
        {
            services.AddSingleton<IPushSubscriptionCosmosDbClient, PushSubscriptionCosmosDbClient>();

            services.AddSingleton<IPushSubscriptionStore, CosmosDbPushSubscriptionStore>();
            services.AddSingleton<IPushSubscriptionStoreAccessorProvider, CosmosDbPushSubscriptionStoreAccessorProvider>();

            return services;
        }
    }
}
