using System;
using Microsoft.Extensions.Configuration;
using WebPush.Interfaces;

namespace WebPush.Services
{
    internal static class ConfigurationExtensions
    {
        private const string SUBSCRIPTION_STORE_TYPE_CONFIGURATION_KEY = "PushSubscriptionStoreType";
        private const string SUBSCRIPTION_STORE_TYPE_SQLITE = "Sqlite";
        private const string SUBSCRIPTION_STORE_TYPE_LITEDB = "LiteDB";
        private const string SUBSCRIPTION_STORE_TYPE_COSMOSDB = "CosmosDB";

        public static SubscriptionStoreTypes GetSubscriptionStoreType(this IConfiguration configuration)
        {
            //string subscriptionStoreType = configuration.GetValue(SUBSCRIPTION_STORE_TYPE_CONFIGURATION_KEY, SUBSCRIPTION_STORE_TYPE_SQLITE);
            return SubscriptionStoreTypes.Sqlite;
        }
    }
}
