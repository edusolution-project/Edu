using BaseEngineEntity;
using Core_v2.Globals;
using FileManagerCore.Globals;
using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineChat
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddLogs();
            services.AddSingleton<IGoogleDriveApiService, GoogleDriveApiService>();
            IGoogleDriveApiService google = (IGoogleDriveApiService)services.BuildServiceProvider().GetService(typeof(IGoogleDriveApiService));
            services.AddRoxyFileManger(google);
            services.AddSingleton<ChatService>();
            services.AddSingleton<ChatDetailService>();
            services.AddSingleton<MessageService>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseCors(builder =>
            {
                builder
                .SetIsOriginAllowed(IsOriginAllowed)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
            app.UseMvc();
        }
        private bool IsOriginAllowed(string host)
        {
            if (host.Contains("localhost")) return true;
            if (host.Contains("eduso.vn")) return true;
            return false;
            //var originConfig = Configuration.GetSection("AllowOrigin").Value;
            //var corsOriginAllowed = new[] { originConfig };
            //return corsOriginAllowed.Any(origin =>Regex.IsMatch(host, $@"^http(s)?://.*{origin}(:[0-9]+)?$", RegexOptions.IgnoreCase));
        }
    }
}
