using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class AccountLogEntity : EntityBase
    {
        [JsonProperty("IP")]
        public string IP { get; set; }
        [JsonProperty("AccountID")]
        public string AccountID { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("Token")]
        public string Token { get; set; }
        [JsonProperty("IsRemember")]
        public bool IsRemember { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class AccountLogService : ServiceBase<AccountLogEntity>
    {
        public AccountLogService(IConfiguration configuration) : base(configuration)
        {

        }
        public AccountLogEntity GetItemByToken(string token)
        {
            return CreateQuery().Find(o => o.Token == token)?.SingleOrDefault();
        }
    }
}
