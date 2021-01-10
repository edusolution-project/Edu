﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using WebPush.Services.Sqlite;

namespace WebPush.Services
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UsePushSubscriptionStore(this IApplicationBuilder app)
        {
            SubscriptionStoreTypes subscriptionStoreType = ((IConfiguration)app.ApplicationServices.GetService(typeof(IConfiguration))).GetSubscriptionStoreType();

            if (subscriptionStoreType == SubscriptionStoreTypes.Sqlite)
            {
                app.UseSqlitePushSubscriptionStore();
            }
            return app;
        }
    }
}
