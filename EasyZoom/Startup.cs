using EasyZoom.Interfaces;
using EasyZoom.Models;
using EasyZoom.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyZoom
{
    public static class Startup
    {
        public static void AddEasyZoom(this IServiceCollection services, IConfiguration config)
        {
            services.AddOptions();
            services.Configure<ZoomConfig>(config.GetSection("ZoomConfig"));
            services.AddSingleton<IZoomHelpers, ZoomHelpers>();
        }
        public static void UseEasyZoom(this IApplicationBuilder app)
        {

        }
    }
}
