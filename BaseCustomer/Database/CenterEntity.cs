using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class CenterEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
    }

    public class CenterService : ServiceBase<CenterEntity>
    {
        public CenterService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CenterEntity>>
            {

            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
