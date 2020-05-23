using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class UserAndRoleEntity : EntityBase
    {
        [JsonProperty("Basis")]
        public string Basis { get; set; }
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("Role")]
        public string Role { get; set; }
    }

    public class UserAndRoleService : ServiceBase<UserAndRoleEntity>
    {
        public UserAndRoleService(IConfiguration config) : base(config)
        {
        }

        public bool SaveRole(UserAndRoleEntity item)
        {
            if(!Exist(item.Basis,item.UserID,item.Role))
            {
                CreateOrUpdate(item);
                return true;
            }
            return false;
        }

        private bool Exist(string basis, string userid, string roleCode)
        {
            var item = CreateQuery().Find(o => o.Basis == basis && o.UserID == userid && o.Role == roleCode)?.ToList();
            return item != null && item.Count > 0;
        }
    }
}
