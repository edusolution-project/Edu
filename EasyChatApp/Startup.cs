using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using EasyChatApp.DataBase;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EasyChatApp
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
            services.AddCors();
            services.AddSignalR();
            services.AddSingleton<EasyChatHub>();
            //services.AddSingleton<IHubContext<EasyChatHub>>();
            services.AddSingleton<GroupUserService>();
            services.AddSingleton<MessagerService>();
            services.AddSingleton<GroupAndUserService>();
            services.AddSingleton<GroupLastLifeService>();
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
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                Path.Combine(env.ContentRootPath, "GoogleDrive")),
                RequestPath = "/GoogleDrive"
            });
            app.UseHttpsRedirection();
            //app.UseCors();
            app.UseCors(builder =>
            {
                builder
                .SetIsOriginAllowed(IsOriginAllowed)
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
            app.UseSignalR(routes =>
            {
                routes.MapHub<EasyChatHub>("/chatHub");
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
