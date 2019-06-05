using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasePublisherModels.Database
{
    public class CPRoleEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Lock { get; set; } = false;
    }
    public class CPRoleService : ServiceBase<CPRoleEntity>
    {
        public CPRoleService(IConfiguration config) : base(config, "CPRoles")
        {

        }

        public CPRoleService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPRoleEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
