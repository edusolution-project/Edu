using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class SysTemplatePropertyEntity : EntityBase
    {
        public string TemplateDetailID { get; set; }  // id templatedetails
        public string Name { get; set; } // key
        public string Value { get; set; } // value
        public string PartialID { get; set; } // vswLogo
    }
    public class SysTemplatePropertyService : ServiceBase<SysTemplatePropertyEntity>
    {
        public SysTemplatePropertyService(IConfiguration config) : base(config, "SysTemplatePropertys")
        {

        }

        public SysTemplatePropertyService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public List<SysTemplatePropertyEntity> GetItemByParentID(string TemplateDetailID)
        {
            return CreateQuery().Find(o => o.TemplateDetailID == TemplateDetailID)?.ToList();
        }
    }
}
