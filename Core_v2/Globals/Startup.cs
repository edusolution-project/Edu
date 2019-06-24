using Core_v2.Interfaces;
using Core_v2.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Core_v2.Globals
{
    public static class Startup
    {
        public static void AddLogs(this IServiceCollection services)
        {
            services.AddTransient<ILog, Log>();
        }
    }
}
