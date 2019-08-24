using BaseHub.Database;
using Microsoft.Extensions.DependencyInjection;

namespace BaseHub.Globals
{
    public static class Instance
    {
        public static IServiceCollection AddServiceHubBase(this IServiceCollection services)
        {
            services.AddSingleton<ChatPrivateService>();
            services.AddSingleton<GroupService>();
            services.AddSingleton<ChatService>();
            services.AddSingleton<NewFeedService>();
            services.AddSingleton<CommentService>();
            return services;
        }
    }
}
