using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SME.Utils.Common
{
    public class CacheManagement
    {
        public static IMemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
    }
}
