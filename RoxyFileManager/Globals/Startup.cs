using BaseCustomerEntity.Database;
using FileManagerCore.Interfaces;
using FileManagerCore.Services;
using GoogleLib.Factory;
using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileManagerCore.Globals
{
    public static class Startup
    {
        public static IServiceCollection AddRoxyFileManger(this IServiceCollection service)
        {
            service.AddSingleton<FolderManagerService>();
            service.AddSingleton<FileManagerService>();
            service.AddSingleton<FolderCenterService>();
            service.AddSingleton<IGoogleDriveApiService, GoogleDriveApiService>();
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
