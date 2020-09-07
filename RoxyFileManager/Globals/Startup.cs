using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileManagerCore.Globals
{
    public static class Startup
    {
        private static IGoogleDriveApiService _googleDriveApiService;
        public static IGoogleDriveApiService GoogleDrive { get { return _googleDriveApiService; } }
        public static IServiceCollection AddRoxyFileManger(this IServiceCollection service, IGoogleDriveApiService googleDriveApiService)
        {
            _googleDriveApiService = googleDriveApiService;
            service.AddSingleton<FolderManagerService>();
            service.AddSingleton<FileManagerService>();
            service.AddSingleton<FolderCenterService>();
            service.AddSingleton<GConfig>();
            service.AddSingleton<IRoxyFilemanHandler, RoxyFilemanHandler>();
            return service;
        }
        public static IApplicationBuilder UseRoxyFileManger(this IApplicationBuilder app)
        {
            return app;
        }
    }
}
