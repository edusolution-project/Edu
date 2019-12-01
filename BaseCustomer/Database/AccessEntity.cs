using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class AccessEntity : EntityBase
    {
        [JsonProperty("CtrlName")]
        public string CtrlName { get; set; }
        [JsonProperty("ActName")]
        public string ActName { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class AccessesService : ServiceBase<AccessEntity>
    {
        public AccessesService(IConfiguration configuration) : base(configuration)
        {

        }
        public IEnumerable<AccessEntity> GetAccessByRole(string RoleID)
        {
            return Collection?.Find(o => o.IsActive == true && o.RoleID == RoleID)?.ToEnumerable();
        }
        
    }
}
