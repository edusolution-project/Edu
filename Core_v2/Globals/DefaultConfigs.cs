using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Cryptography;
using System.Text;

namespace Core_v2.Globals
{
    public class DefaultConfigs
    {
        public string defaultAvatar { get; set; } = "<svg width='24' height='24' viewBox='0 0 24 24' fill='none' xmlns='http://www.w3.org/2000/svg'><path d='M24 12C24 18.6275 18.6275 24 12 24C5.3725 24 0 18.6275 0 12C0 5.3725 5.3725 0 12 0C18.6275 0 24 5.3725 24 12Z' fill='#7CC6F1'/><path d = 'M14.7283 11.6221C15.8306 10.7919 16.5447 9.47278 16.5447 7.98981C16.5447 5.48401 14.506 3.44531 12 3.44531C9.4942 3.44531 7.45551 5.48401 7.45551 7.98981C7.45551 9.47278 8.16943 10.7919 9.27173 11.6221C6.32043 12.7299 4.21417 15.5804 4.21417 18.9141H5.62042C5.62042 15.3962 8.48236 12.5345 12.0002 12.5345C15.5178 12.5345 18.3798 15.3962 18.3798 18.9141H19.786C19.7858 15.5804 17.6797 12.7299 14.7283 11.6221ZM8.86176 7.98981C8.86176 6.25928 10.2695 4.85156 12 4.85156C13.7305 4.85156 15.1382 6.25928 15.1382 7.98981C15.1382 9.72034 13.7305 11.1282 12 11.1282C10.2695 11.1282 8.86176 9.72034 8.86176 7.98981Z' fill='white'/></svg>";
        public float avatar_max_size_mb { get; set; }
        public string defaultClassID { get; set; }
    }

}
