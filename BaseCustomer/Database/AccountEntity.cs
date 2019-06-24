using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class AccountEntity: EntityBase
    {
        public string UserID { get; set; }
        public string Type { get; set; } // admin/student/teacher
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string PassTemp { get; set; }
        public string RoleID { get; set; }
        public bool IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class AccountService : ServiceBase<AccountEntity>
    {
        public AccountService(IConfiguration configuration) : base(configuration)
        {

        }
        public AccountEntity GetAccount(string type,string userName,string passWord)
        {
            var pass = Core_v2.Globals.Security.Encrypt(passWord);
            return Collection.Find(o => o.Type == type && o.UserName == userName && (o.PassWord == pass || o.PassTemp == pass))?.SingleOrDefault();
        }
    }
}
