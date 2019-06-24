using Core_v2.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Core_v2.Repositories
{
    public class ServiceBase<T> : IServiceBase<T> where T : EntityBase, new()
    {
        private readonly string _tableName;
        private readonly IConfiguration _config;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        public ServiceBase(IConfiguration config, string tableName, string dbName = "")
        {
            _config = config;
            _tableName = tableName;
            if(string.IsNullOrEmpty(dbName)) dbName = _config.GetSection("dbName:Default").Value;
            _client = new MongoClient(_config.GetConnectionString(dbName));
            _database = _client.GetDatabase(dbName);
            _collection = _database.GetCollection<T>(tableName);
        }
        public ServiceBase(IConfiguration config, string dbName = "")
        {
            _config = config;
            if (string.IsNullOrEmpty(dbName)) dbName = _config.GetSection("dbName:Default").Value;
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
        public T GetItemByID(string id)
        {
            return _collection.Find(o => o.ID == id)?.SingleOrDefault();
        }
        public IFindFluent<T, T> GetAll(){
            return _collection.Find(_ => true);
        } 
    }
}
