using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ModSubjectEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
    public class ModSubjectService : ServiceBase<ModSubjectEntity>
    {
        public ModSubjectService(IConfiguration config) : base(config, "ModSubject", config.GetSection("dbName:Publisher").Value)
        {
        }
        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
