using CoreMongoDB.Interfaces;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CoreMongoDB.Repositories
{
    public class DbQueryCache: IDbQueryCache
    {
        const int DefaultExpire = 240;

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
        /// set cache
        /// </summary>
        /// <typeparam name="T"> kieu du lieu </typeparam>
        /// <param name="nameCache"> key </param>
        /// <param name="value"> du lieu </param>
        public void SetObjectFromCache<T>(string nameCache, T value)
        {
            ObjectCache cache = MemoryCache.Default;
            var cachedObject = value;
            CacheItemPolicy policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(DefaultExpire)
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
    public static class DbQuery
    {
        private static readonly IDbQueryCache _dbQueryCache = new DbQueryCache();
        public static List<TDocument> ToListCache<TDocument>(this IAsyncCursor<TDocument> source)
        {
            string key = "List" + typeof(TDocument).Name.Replace("entity", string.Empty);
            var data = _dbQueryCache.GetDataFromCache<List<TDocument>>(key);
            if (data != null) return data;
            else
            {
                data = source.ToList();
                _dbQueryCache.SetObjectFromCache(key, data);
                return data;
            }

        }
        public static List<TDocument> ToListCache<TDocument>(this IAsyncCursor<TDocument> source,string key)
        {
            var data = _dbQueryCache.GetDataFromCache<List<TDocument>>(key);
            if (data != null) return data;
            else
            {
                data = source.ToList();
                _dbQueryCache.SetObjectFromCache(key, data);
                return data;
            }
        }
        public static List<TDocument> ToListCache<TDocument>(this IAsyncCursor<TDocument> source, string key,int expire)
        {
            var data = _dbQueryCache.GetDataFromCache<List<TDocument>>(key);
            if (data != null) return data;
            else
            {
                data = source.ToList();
                _dbQueryCache.SetObjectFromCache(key,expire,data);
                return data;
            }
        }
        public static Task<IAsyncCursor<TDocument>> FindAsync<TDocument>(this IMongoCollection<TDocument> collection,bool check, Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null)
        {
            if (check)
            {

                return options != null ? collection.FindAsync(filter, options) : collection.FindAsync(filter);
            }
            else
            {
                return options != null ? collection.FindAsync(o=>o != null,options) : collection.FindAsync(o => o != null);
            }
        }
        public static IFindFluent<TDocument, TDocument> Find<TDocument>(this IMongoCollection<TDocument> collection,bool check, Expression<Func<TDocument, bool>> filter, FindOptions options = null)
        {
            if (check)
            {
                return options == null ? collection.Find(filter) : collection.Find(filter, options);
            }
            else
            {
                return options == null ? collection.Find(o => o != null) : collection.Find(o => o != null, options);
            }
        }
        public static List<TDocument> FindList<TDocument>(this IMongoCollection<TDocument> collection, bool check, Expression<Func<TDocument, bool>> predicate, FindOptions options = null)
        {
            if (check)
            {
                return options == null ? collection.Find(predicate)?.ToList() : collection.Find(predicate, options)?.ToList();
            }
            else
            {
                return options == null ? collection.Find(o => o != null)?.ToList() : collection.Find(o => o != null, options)?.ToList();
            }
        }
    }
}
