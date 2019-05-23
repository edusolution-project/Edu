using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SME.Utils.Common
{
    public static class ConfigurationManager
    {
        public static IConfiguration AppSettings { get; }
        static ConfigurationManager()
        {
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //.AddJsonFile("appsettings.json")
        //.Build();
            AppSettings = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();
        }
    }
}
