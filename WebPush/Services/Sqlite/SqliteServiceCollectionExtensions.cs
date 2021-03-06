﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebPush.Interfaces;
using WebPush.Services.Sqlite;

namespace WebPush.Services.Sqlite
{
    public static class SqliteServiceCollectionExtensions
    {
        private const string SQLITE_CONNECTION_STRING_NAME = "PushSubscriptionSqliteDatabase";

        public static IServiceCollection AddSqlitePushSubscriptionStore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<PushSubscriptionContext>(options =>
                options.UseSqlite(configuration.GetConnectionString(SQLITE_CONNECTION_STRING_NAME))
            );

            services.AddTransient<IPushSubscriptionStore, SqlitePushSubscriptionStore>();
            services.AddHttpContextAccessor();
            services.AddSingleton<IPushSubscriptionStoreAccessorProvider, SqlitePushSubscriptionStoreAccessorProvider>();

            return services;
        }
    }
}
