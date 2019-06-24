using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class AccountLogEntity : EntityBase
    {
        public string IP { get; set; }
        public string AccountID { get; set; }
        public string Type { get; set; }
        public string Token { get; set; }
        public bool IsRemember { get; set; }
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
