using System;
using System.Runtime.Caching;
using System.Linq;

namespace BaseMVC.Globals
{
    public static class CacheExtends
    {
        public const string DefaultIsControl = "CPControl";
        public const string DefaultPermission = "Permission";
        public const string DefaultLang = "Language";
        public const string DefaultResource = "Resource";
        public const string DefaultAdminController = "CPMenu";
        public const string DefaultClientController = "ClientMenu";

        #region cache
        /// <summary>
        /// get cache
        /// </summary>
        /// <typeparam name="T">kieu du lieu</typeparam>
        /// <param name="nameCache">key</param>
        /// <returns></returns>
        public static T GetDataFromCache<T>(string nameCache)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = (T)cache[nameCache];
            return cachedObject;
        }
        /// <summary>
        /// set cache
        /// </summary>
        /// <typeparam name="T"> kieu du lieu </typeparam>
        /// <param name="nameCache"> key </param>
        /// <param name="expire"> thoi gia luu phut </param>
        /// <param name="value"> du lieu </param>
        public static void SetObjectFromCache<T>(string nameCache, int expire, T value)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = value;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(expire)
            };
            cache.Set(nameCache, cachedObject, policy);
            
        }
        /// <summary>
        /// clear cache chi dinh 
        /// </summary>
        /// <param name="keyName">key</param>
        public static void ClearCache(string keyName)
        {
            ObjectCache cache = MemoryCache.Default;
            var obj = cache.Get(keyName);
            if (obj != null)
            {
                cache.Remove(keyName);
            }
        }
        /// <summary>
        /// xoas tat ca cache
        /// </summary>
        public static void ClearCacheAll()
        {
            var cache = MemoryCache.Default;
            if (cache.Count() == 0) return;
            foreach (var item in cache)
            {
                cache.Remove(item.Key);
            }
        }
        #endregion
    }
}
