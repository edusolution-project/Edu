using BaseEasyRealTime.Entities;
using FileManagerCore.Globals;
using Microsoft.Extensions.DependencyInjection;

namespace BaseEasyRealTime.Globals
{
    public static class Startup
    {
        /// <summary>
        /// dùng AddRoxyFileManger trức khi thêm AddEasyRealTime
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public static IServiceCollection AddEasyRealTime(this IServiceCollection service)
        {
            service.AddRoxyFileManger();
            service.AddSingleton<GroupService>();
            service.AddSingleton<MessageService>();
            service.AddSingleton<NewFeedService>();
            service.AddSingleton<CommentService>();
            service.AddSingleton<NotificationService>();
            return service;
        }
    }
}
