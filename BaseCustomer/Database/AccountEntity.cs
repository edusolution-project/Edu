using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class AccountEntity : EntityBase
    {
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; } // admin/student/teacher
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("PassWord")]
        public string PassWord { get; set; }
        [JsonProperty("PassTemp")]
        public string PassTemp { get; set; }
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class AccountService : ServiceBase<AccountEntity>
    {
        public AccountService(IConfiguration configuration) : base(configuration)
        {

        }


        public AccountEntity GetAccount(string type, string userName, string passWord)
        {
            var pass = passWord;
            return Collection.Find(o => o.Type == type && o.UserName == userName && (o.PassWord == pass || o.PassTemp == pass))?.SingleOrDefault();
        }

        [Obsolete]
        public bool IsAvailable(string userName)
        {
            return Collection.Count(o => o.UserName == userName) > 0;
        }
    }
}
