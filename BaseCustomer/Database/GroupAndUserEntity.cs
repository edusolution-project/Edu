using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class GroupAndUserEntity : EntityBase
    {
        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("TimeJoin")]
        public long TimeJoin { get; set; }
        [JsonProperty("TimeLife")]
        public long TimeLife { get; set; }
    }
    public class GroupAndUserService : ServiceBase<GroupAndUserEntity>
    {
        public GroupAndUserService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
