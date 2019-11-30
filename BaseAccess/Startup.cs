using BaseAccess.Attribule;
using BaseAccess.Interfaces;
using BaseAccess.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BaseAccess
{
    public static class Startup
    {
        public static void AddAccess(this IServiceCollection service)
        {
            service.AddSingleton<AccessCtrlAttribute>();
            service.AddSingleton<IAccess, AccessService>();
            service.AddSingleton<IAuthenService, AuthenService>();
        }
    }
}
