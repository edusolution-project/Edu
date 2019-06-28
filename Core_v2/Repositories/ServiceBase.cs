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
            string _dbname = "";
            if(string.IsNullOrEmpty(dbName)) _dbname = _config.GetSection("dbName:Default").Value;
            else _dbname = _config.GetSection("dbName:"+dbName).Value;
            _client = new MongoClient(_config.GetConnectionString(_dbname));
            _database = _client.GetDatabase(_dbname);
            _collection = _database.GetCollection<T>(tableName);
        }
        public ServiceBase(IConfiguration config, string dbName = "")
        {
            _config = config;
            string _dbname = "";
            if (string.IsNullOrEmpty(dbName)) _dbname = _config.GetSection("dbName:Default").Value;
            else _dbname = _config.GetSection("dbName:" + dbName).Value;
            _tableName = typeof(T).Name.ToLower().Replace("entity", string.Empty).Replace("model", string.Empty);
            _client = new MongoClient(_config.GetConnectionString(_dbname));
            _database = _client.GetDatabase(_dbname);
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
