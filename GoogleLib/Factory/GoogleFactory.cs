using GoogleLib.Interfaces;
using GoogleLib.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleLib.Factory
{
    public class GoogleFactory
    {
        public static IGoogleDriveApiService GetGoogleDrive()
        {
            return new GoogleDriveApiService();
        }
    }
}
