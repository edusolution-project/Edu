using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasePublisherModels.Database
{
    public class CPUserEntity : EntityBase
    {
        public string Name { get; set; }
        public string RoleID { get; set; }
        public string Email { get; set; }
        public string Pass { get; set; }
        public DateTime BirthDay { get; set; }
        public string Skype { get; set; }
        public string Phone { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class CPUserService : ServiceBase<CPUserEntity>
    {
        public CPUserService(IConfiguration config) : base(config, "CPUsers")
        {

        }

        public CPUserService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPUserEntity GetItemByEmail(string email)
        {
            return CreateQuery().Find(o => o.Email == email)?.SingleOrDefault();
        }
    }
}
