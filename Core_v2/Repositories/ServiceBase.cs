using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Core_v2.Repositories
{
    public class ServiceBase<T> : IServiceBase<T> where T : EntityBase, new()
    {
        private readonly string _tableName;
        private readonly IConfiguration _config;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        private readonly MappingEntity<T, T> _mapping;
        public ServiceBase(IConfiguration config, string tableName, string dbName = "")
        {
            _config = config;
            _tableName = tableName;
            string _dbname = "";
            string _connectionName = "";
            if (string.IsNullOrEmpty(dbName))
            {
                _connectionName = _config.GetSection("dbName:Default").Value;
                _dbname = _config.GetSection("dbName:Default").Value;
            }
            else
            {
                _connectionName = dbName;
                _dbname = dbName;
            }
            /* _config.GetSection("dbName:"+dbName).Value;*/
            _client = new MongoClient(_config.GetConnectionString(_connectionName));
            _database = _client.GetDatabase(_dbname);
            _collection = _database.GetCollection<T>(tableName);
            _mapping = new MappingEntity<T, T>();
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
            _mapping = new MappingEntity<T, T>();
        }

        public virtual IMongoCollection<T> Collection
        {
            get
            {
                return _collection ?? _database.GetCollection<T>(_tableName);
            }
        }

        public MappingEntity<T, T> Map
        {
            get
            {
                return _mapping;
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

        public IFindFluent<T, T> GetAll()
        {
            return _collection.Find(_ => true);
        }

        public T CreateOrUpdate(T item)
        {
            if (string.IsNullOrEmpty(item.ID) || item.ID == "0")
            {
                _collection.InsertOne(item);
                return item;
            }
            else
            {
                var oldItem = _collection.Find(o => o.ID == item.ID).FirstOrDefault();
                if (oldItem != null)
                {
                    var newItem = _mapping.Auto(oldItem, item);//stupid mapping
                    _collection.ReplaceOne(o => o.ID == newItem.ID, newItem);
                    return newItem;
                }
                else
                {
                    return null;
                }
            }
        }
        public DeleteResult Remove(string ID)
        {
            if (string.IsNullOrEmpty(ID) || ID == "0")
            {
                return null;
            }
            else
            {
                var oldItem = _collection.Find(o => o.ID == ID).First();
                if (oldItem != null)
                {
                    return _collection.DeleteOne(o => o.ID == ID);
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task RemoveAsync(string ID)
        {
            if (string.IsNullOrEmpty(ID) || ID == "0")
                return;
            await _collection.DeleteManyAsync(t => t.ID == ID);
        }
    }
}
