using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class GroupLastLifeEntity : EntityBase
    {
        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
        [JsonProperty("TimeLife")]
        public long TimeLife { get; set; }
    }
    public class GroupLastLifeService : ServiceBase<GroupLastLifeEntity>
    {
        public GroupLastLifeService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
