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
    {
        public string key { get; set; }
        public double index { get; set; } = 0;
    }
    public class IndexService : ServiceBase<IndexEntity>
    {
        public IndexService(IConfiguration config) : base(config)
        {
        }
        public double GetNewIndex(string name)
        {
            var option = new FindOneAndUpdateOptions<IndexEntity, IndexEntity> { IsUpsert = true };
            var result = Collection.FindOneAndUpdate<IndexEntity>(o => o.key == name,
                                               Builders<IndexEntity>.Update.Inc("index", 1),
                                               option);
            if (result == null)
            {
                Collection.InsertOne(new IndexEntity { key = name, index = 1 });
                return 1;
            }
            return result.index;
        }
    }
}
