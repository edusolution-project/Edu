using BaseCustomerEntity.Database;
using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileManagerCore.Globals
{
    public static class Startup {
        public static IConfiguration Configuration;
    
        private static IGoogleDriveApiService _driveApiService;
        public static IGoogleDriveApiService GetGoogleApi()
        {
            if (_driveApiService == null)
            {
                _driveApiService = new GoogleDriveApiService(Configuration);
            }
            return _driveApiService;
        }
        public static IServiceCollection AddRoxyFileManger(this IServiceCollection service, IConfiguration configuration)
        {
            Configuration = configuration;
            service.AddSingleton<FolderManagerService>();
            service.AddSingleton<FileManagerService>();
            service.AddSingleton<FolderCenterService>();
            service.AddSingleton<IGoogleDriveApiService, GoogleDriveApiService>();
            service.AddSingleton<GConfig>();
            service.AddSingleton<IRoxyFilemanHandler, RoxyFilemanHandler>();
            if(_driveApiService == null)
            {
                _driveApiService = new GoogleDriveApiService(configuration);
            }
            return service;
        }
        public static IApplicationBuilder UseRoxyFileManger(this IApplicationBuilder app)
        {

            return app;
        }
    }
}
