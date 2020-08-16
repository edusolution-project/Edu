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
        public static IServiceCollection AddRoxyFileManger(this IServiceCollection service)
        {
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
