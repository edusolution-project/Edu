using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class RoleEntity: EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
    }
    public class RoleService : ServiceBase<RoleEntity>
    {
        public RoleService(IConfiguration configuration) : base(configuration)
        {

        }
        public RoleEntity GetItemByCode(string code)
        {
            return Collection.Find(o => o.Code == code)?.First();
        }
    }
}
