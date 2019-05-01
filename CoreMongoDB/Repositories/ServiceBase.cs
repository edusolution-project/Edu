using CoreMongoDB.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreMongoDB.Repositories
{
    public class ServiceBase<T> : DbContextHelper<T>, IServiceBase<T> where T : EntityBase, new()
    {
        private readonly string _tableName;
        public const string defaultConn = "DefaultConn";
        private readonly IDbQueryCache _dbQueryCache;
        public ServiceBase(IConfiguration config) : base(config,defaultConn)
        {
            _tableName = typeof(T).Name.ToLower().Replace("entity", string.Empty).Replace("model", string.Empty);
            _dbQueryCache = new DbQueryCache();

        }
        public ServiceBase(IConfiguration config, string tableName) : base(config, tableName, defaultConn)
        {
            _tableName = tableName;
            _dbQueryCache = new DbQueryCache();
        }
        public ServiceBase(IConfiguration config, string tableName,string connStr = defaultConn) : base(config, tableName, connStr)
        {
            _tableName = tableName;
            _dbQueryCache = new DbQueryCache();
        }
        public void Add(T item)
        {
            var data = Collection.Find(o => o.ID == item.ID).SingleOrDefault();
            if (data == null)
            {
                Collection.InsertOne(item);
            }
            else
            {
                Collection.FindOneAndReplace(o => o.ID == item.ID, item);
            }
        }
        public void Update(Expression<Func<T, bool>> expression, T item)
        {
            var data = string.IsNullOrEmpty(item.ID)
                ? Collection.Find(expression)?.SingleOrDefault()
                : Collection.Find(o => o.ID == item.ID)?.SingleOrDefault();
            if (data != null)
            {
                UpdateDefinition<T> update = null;
                BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(item.ToBson());

                foreach (BsonElement element in document)
                {
                    update = update?.Set(e => e[element.Name], element.Value) ?? Builders<T>.Update.Set(e => e[element.Name], element.Value);
                }
                if (update != null)
                {
                    Collection.UpdateOne(expression, update);
                }
            }
        }
        public Task UpdateAsync(Expression<Func<T, bool>> expression, T item)
        {
            var data = string.IsNullOrEmpty(item.ID)
                ? Collection.Find(expression)?.SingleOrDefault()
                : Collection.Find(o => o.ID == item.ID)?.SingleOrDefault();
            if (data != null)
            {
                UpdateDefinition<T> update = null;
                BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(item.ToBson());

                foreach (BsonElement element in document)
                {
                    update = update?.Set(e => e[element.Name], element.Value) ?? Builders<T>.Update.Set(e => e[element.Name], element.Value);
                }
                if (update != null)
                {
                    Collection.UpdateOneAsync(expression, update);
                }
            }
            return Task.CompletedTask;
        }

        public async Task AddAsync(T item)
        {
            var data = Collection.Find(o => o.ID == item.ID).SingleOrDefault();
            if (data == null)
            {
                await Collection.InsertOneAsync(item);
            }
            else
            {
                await Collection.FindOneAndReplaceAsync(o => o.ID == item.ID, item);
            }
        }

        public void AddRange(IEnumerable<T> listItem)
        {
            Collection.InsertMany(listItem);
        }

        public async Task AddRangeAsync(IEnumerable<T> listItem)
        {
            await Collection.InsertManyAsync(listItem);
        }

        public IEnumerable<T> GetAll()
        {
            return Collection.Find(o => !string.IsNullOrEmpty(o.ID))?.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var data = await Collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
            return data?.ToList();
        }

        public T GetByID(string ID)
        {
            var data = Collection.Find(o => o.ID == ID).SingleOrDefault();
            return data ?? null;
        }

        public async Task<T> GetByIDAsync(string ID)
        {
            var data = await Collection.FindAsync(o => o.ID == ID);
            return data?.SingleOrDefault();
        }

        public void Remove(string ID)
        {
            Collection.DeleteOne(o => o.ID == ID);
        }

        public async Task RemoveAsync(string ID)
        {
            await Collection.DeleteOneAsync(o => o.ID == ID);
        }

        public void RemoveRange(IEnumerable<string> listItem)
        {
            Collection.DeleteMany(o => listItem.Contains(o.ID));
        }

        public async Task RemveRangeAsync(IEnumerable<string> listItem)
        {
            await Collection.DeleteManyAsync(o => listItem.Contains(o.ID));
        }

        public IEnumerable<T> Find(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                return Collection.Find(filter)?.ToEnumerable();
            }
            else
            {
                return Collection.Find(o => !string.IsNullOrEmpty(o.ID))?.ToEnumerable();
            }
        }
        public IEnumerable<T> FindIn(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                return Collection.Find(filter)?.ToList();
            }
            else
            {
                return Collection.Find(o => !string.IsNullOrEmpty(o.ID))?.ToList();
            }
        }
        public async Task<IEnumerable<T>> WhereAsync(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                var data = await Collection.FindAsync(filter);
                return data?.ToEnumerable();
            }
            else
            {
                var data = await Collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
                return data?.ToEnumerable();
            }
        }
        public async Task<IEnumerable<T>> FindInAsync(bool check, Expression<Func<T, bool>> filter)
        {
            if (check)
            {
                var data = await Collection.FindAsync(filter);
                return data?.ToList();
            }
            else
            {
                var data = await Collection.FindAsync(o => !string.IsNullOrEmpty(o.ID));
                return data?.ToList();
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
        public void SetItemCache(string key, T item)
        {
            _dbQueryCache.SetObjectFromCache<T>(key, item);
        }
        public void SetListCache(string key, IEnumerable<T> item)
        {
            _dbQueryCache.SetObjectFromCache(key, item);
        }
        public IEnumerable<T> GetListCache()
        {
            return _dbQueryCache.GetDataFromCache<IEnumerable<T>>("List" + _tableName);
        }
        public IEnumerable<T> GetListCache(string key)
        {
            return _dbQueryCache.GetDataFromCache<IEnumerable<T>>(key);
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
