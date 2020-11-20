using BaseCustomerEntity.Database;
using Core_v2.Globals;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class CacheHelper
    {
        private readonly IDistributedCache _cache;
        private readonly string cachePrefix;
        private readonly TimeSpan defTimespan = TimeSpan.FromSeconds(3600);

        public CacheHelper(IConfiguration iConfig,
            IDistributedCache cache
        )
        {
            _cache = cache;
            cachePrefix = iConfig.GetValue<string>("CacheConfig:PrefixKey");
        }

        public void SetCache(string key, object obj, TimeSpan? ts = null)
        {
            key = cachePrefix + "_" + key;
            _cache.SetAsync(key, ObjectToByteArray(obj), new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = ts ?? defTimespan });
        }

        public void SetCache(string key, object obj, DistributedCacheEntryOptions opt)
        {
            key = cachePrefix + "_" + key;
            _cache.SetAsync(key, ObjectToByteArray(obj), opt);
        }

        public void ClearCache(string key)
        {
            key = cachePrefix + "_" + key;
            _cache.RemoveAsync(key);
        }

        public Object GetCache(string key)
        {
            key = cachePrefix + "_" + key;
            var cacheVal = _cache.Get(key);
            if (cacheVal == null) return null;
            return ByteArrayToObject(cacheVal);
        }

        private byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(arrBytes, 0, arrBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)bf.Deserialize(ms);
            return obj;
        }

    }
}
