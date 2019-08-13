using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using MongoDB;

namespace BaseCustomerEntity.Database
{
    public class IndexEntity : EntityBase
    {        public string key { get; set; }
        public int index { get; set; } = 0;
    }
    public class IndexService : ServiceBase<IndexEntity>
    {
        public IndexService(IConfiguration config) : base(config)
        {
        }
        public int GetNewIndex(string name)
        {
            return Collection.FindOneAndUpdate(o => o.key == name, Builders<IndexEntity>.Update.Inc("index", 1)).index;
        }
    }
}
