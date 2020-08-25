using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class GrpupLastLifeEntity : EntityBase
    {
        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
        [JsonProperty("TimeLife")]
        public long TimeLife { get; set; }
    }
    public class GrpupLastLifeService : ServiceBase<GrpupLastLifeEntity>
    {
        public GrpupLastLifeService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
