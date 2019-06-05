using CoreEDB.Interfaces;
using System;
using System.Linq;
using System.Runtime.Caching;

namespace CoreEDB.Repositories
{
    public class DbQueryCache : IDbQueryCache
    {
        public const int Expires = 240;
        public DbQueryCache()
        {

        }
        #region cache
        /// <summary>
        /// get cache
        /// </summary>
        /// <typeparam name="T">kieu du lieu</typeparam>
        /// <param name="nameCache">key</param>
        /// <returns></returns>
        public T GetDataFromCache<T>(string nameCache)
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
        public void SetObjectFromCache<T>(string nameCache, int expire, T value)
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
        public void ClearCache(string keyName)
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
        public void ClearCacheAll()
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
