using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class CPUserSubEntity : EntityBase
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
    public class CPUserSubService : ServiceBase<CPUserSubEntity>
    {
        public CPUserSubService(IConfiguration config) : base(config, "CPUsersSub")
        {

        }

        public CPUserSubService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPUserSubEntity GetItemByEmail(string email)
        {
            return CreateQuery().Find(o => o.Email == email)?.SingleOrDefault();
        }
    }
}
