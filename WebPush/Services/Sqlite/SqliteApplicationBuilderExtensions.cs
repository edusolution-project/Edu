using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebPush.Services.Sqlite
{
    public static class SqliteApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSqlitePushSubscriptionStore(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
