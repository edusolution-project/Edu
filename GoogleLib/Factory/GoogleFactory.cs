using GoogleLib.Interfaces;
using GoogleLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleLib.Factory
{
    public class GoogleFactory
    {
        public static IGoogleDriveApiService GetGoogleDrive(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            return new GoogleDriveApiService(configuration, hostingEnvironment);
        }
    }
}
