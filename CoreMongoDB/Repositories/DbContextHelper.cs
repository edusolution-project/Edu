using CoreMongoDB.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace CoreMongoDB.Repositories
{
    public abstract class DbContextHelper<T> : IDbContextHelper<T> where T : EntityBase
    {
        private readonly string _tableName;
        private readonly IConfiguration _config;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;

        public DbContextHelper(IConfiguration config, string tableName, string dbName)
        {
            _config = config;
            _tableName = tableName;
            _client = new MongoClient(_config.GetConnectionString(dbName));
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<T>(tableName);
        }
        public DbContextHelper(IConfiguration config,string dbName)
        {
            _config = config;
            _tableName = typeof(T).Name.ToLower().Replace("entity", string.Empty).Replace("model", string.Empty);
            _client = new MongoClient(_config.GetConnectionString(dbName));
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<T>(_tableName);
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
    }
}
