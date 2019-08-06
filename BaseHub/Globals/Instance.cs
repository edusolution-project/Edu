using Microsoft.Extensions.DependencyInjection;

namespace BaseHub.Globals
{
    public static class Instance
    {
        public static IServiceCollection AddServiceBase(this IServiceCollection services)
        {
            //services.AddSingleton<>();
            return services;
        }
    }
}
