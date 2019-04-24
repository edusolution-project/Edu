using CoreMongoDB.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreMongoDB.Repositories
{
    public class DbContextHelper<T> : IDbContextHelper<T> where T : EntityBase
    {
        private readonly string _tableName;
        private readonly IConfiguration _config;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private readonly IDbQueryCache _dbQueryCache;
        public DbContextHelper(IConfiguration config, string tableName, string dbName)
        {
            _config = config;
            _tableName = tableName;
            _client = new MongoClient(_config.GetConnectionString(dbName));
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<T>(tableName);
            _dbQueryCache = new DbQueryCache();
        }
        public DbContextHelper(IConfiguration config,string dbName)
        {
            _config = config;
            _tableName = typeof(T).Name.ToLower().Replace("entity",string.Empty).Replace("model", string.Empty);
            _client = new MongoClient(_config.GetConnectionString(dbName));
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<T>(_tableName);
            _dbQueryCache = new DbQueryCache();
        }
        public virtual IMongoCollection<T> Collection
        {
            get
            {
                return _collection ?? _database.GetCollection<T>(_tableName);
            }
        }

        public virtual IMongoCollection<T> CreateQuery()
        {
            return _collection ?? _database.GetCollection<T>(_tableName);
        }
        public void Add(T item)
        {
            var data = _collection.Find(o => o.ID == item.ID).SingleOrDefault();
            if(data == null)
            {
                _collection.InsertOne(item);
            }
            else
            {
                _collection.FindOneAndReplace(o => o.ID == item.ID, item);
            }
        }

        public async Task AddAsync(T item)
        {
            var data = _collection.Find(o => o.ID == item.ID).SingleOrDefault();
            if (data == null)
            {
                await _collection.InsertOneAsync(item);
            }
            else
            {
                await _collection.FindOneAndReplaceAsync(o => o.ID == item.ID, item);
            }
        }

        public void AddRange(List<T> listItem)
        {
            _collection.InsertMany(listItem);
        }

        public async Task AddRangeAsync(List<T> listItem)
        {
            await _collection.InsertManyAsync(listItem);
        }

        public List<T> GetAll()
        {
            return _collection.Find(o => !string.IsNullOrEmpty(o.ID))?.ToList();
        }

        public async Task<List<T>> GetAllAsync()
        {
            var data = await _collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
            return data?.ToList();
        }

        public T GetByID(string ID)
        {
            var data = _collection.Find(o => o.ID == ID).SingleOrDefault();
            return data ?? null;
        }

        public async Task<T> GetByIDAsync(string ID)
        {
            var data = await _collection.FindAsync(o => o.ID == ID);
            return data?.SingleOrDefault();
        }

        public void Remove(string ID)
        {
            _collection.DeleteOne(o => o.ID == ID);
        }

        public async Task RemoveAsync(string ID)
        {
            await _collection.DeleteOneAsync(o => o.ID == ID);
        }

        public void RemoveRange(List<string> listItem)
        {
            _collection.DeleteMany(o => listItem.Contains(o.ID));
        }

        public async Task RemveRangeAsync(List<string> listItem)
        {
            await _collection.DeleteManyAsync(o => listItem.Contains(o.ID));
        }

        public IEnumerable<T> Find(bool check,Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                return _collection.Find(filter)?.ToEnumerable();
            }
            else
            {
                return _collection.Find(o => !string.IsNullOrEmpty(o.ID))?.ToEnumerable();
            }
        }
        public IFindFluent<T,T> FindIn(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                return _collection.Find(filter);
            }
            else
            {
                return _collection.Find(o => !string.IsNullOrEmpty(o.ID));
            }
        }
        public async Task<IEnumerable<T>> WhereAsync(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                var data = await _collection.FindAsync(filter);
                return data?.ToEnumerable();
            }
            else
            {
                var data = await _collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
                return data?.ToEnumerable();
            }
        }
        public async Task<IAsyncCursor<T>> FindInAsync(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                var data = await _collection.FindAsync(filter);
                return data;
            }
            else
            {
                return await _collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
            }
        }

        public void SetItemCache(T item)
        {
            _dbQueryCache.SetObjectFromCache<T>("Single" + _tableName, item);
        }
        public void SetListCache(List<T> item)
        {
            _dbQueryCache.SetObjectFromCache("List" + _tableName, item);
        }
        public void SetItemCache(string key,T item)
        {
            _dbQueryCache.SetObjectFromCache<T>(key, item);
        }
        public void SetListCache(string key,List<T> item)
        {
            _dbQueryCache.SetObjectFromCache(key, item);
        }
        public List<T> GetListCache()
        {
            return _dbQueryCache.GetDataFromCache<List<T>>("List" + _tableName);
        }
        public List<T> GetListCache(string key)
        {
            return _dbQueryCache.GetDataFromCache<List<T>>(key);
        }
        public T GetItemCache()
        {
            return _dbQueryCache.GetDataFromCache<T>("Single" + _tableName);
        }
        public T GetItemCache(string key)
        {
            return _dbQueryCache.GetDataFromCache<T>(key);
        }
        public void ClearCache()
        {
            _dbQueryCache.ClearCache("List" + _tableName);
            _dbQueryCache.ClearCache("Single" + _tableName);
        }
    }
}
