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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BaseEasyRealTime.Globals;

namespace EnglishPlatform
{
    public class Program
    {
        private static IGoogleDriveApiService _googleDriveApiService;
        public static IGoogleDriveApiService GoogleDriveApiService
        {
            get
            {
                return _googleDriveApiService;
            }
        } 
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, configBuilder) =>
            {
                var config = configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                if (_googleDriveApiService == null && config != null)
                {
                    _googleDriveApiService = new GoogleDriveApiService(config.Build());
                }
            }).ConfigureServices(services =>
            {
                services.AddEasyRealTime(GoogleDriveApiService);
            })
            //.UseHttpSys(options => { options.MaxRequestBodySize = 100_000_000; })
            //.UseKestrel(options => { options.Limits.MaxRequestBodySize = null; })
            .UseStartup<Startup>();
    }
}
