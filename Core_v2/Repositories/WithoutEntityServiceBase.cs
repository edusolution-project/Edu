﻿using Core_v2.Globals;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Core_v2.Repositories
{
    public class WithoutEntityServiceBase<T> where T : class,new()
    {
        private readonly string _tableName;
        private readonly IConfiguration _config;
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<T> _collection;
        public WithoutEntityServiceBase(IConfiguration config, string tableName, string dbName = "")
        {
            _config = config;
            _tableName = tableName;
            string _dbname, _connectionName;
            if (string.IsNullOrEmpty(dbName))
            {
                _connectionName = _config.GetSection("dbName:Default").Value;
                _dbname = string.IsNullOrEmpty(Startup.keyOver) ? _config.GetSection("dbName:Default").Value : Startup.keyOver;
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
        }

        public WithoutEntityServiceBase(IConfiguration config, string dbName = "")
        {
            _config = config;
            string _dbname = "";
            if (string.IsNullOrEmpty(dbName)) { _dbname = string.IsNullOrEmpty(Startup.keyOver) ? _config.GetSection("dbName:Default").Value : Startup.keyOver; }
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
    }
}
