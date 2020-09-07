using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GoogleApp
{
    public class Program
    {
        public static IGoogleDriveApiService GoogleDriveApiService { get; private set; }
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                if (GoogleDriveApiService == null && config != null)
                {
                    GoogleDriveApiService = new GoogleDriveApiService(config.Build());
                }
            }).UseStartup<Startup>();
    }
}
