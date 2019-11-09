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
        public string defaultAvatar { get; set; }
        public float avatar_max_size_mb { get; set; }

    }

}
