using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class SysPageEntity : EntityBase
    {
        public string ParentID { get; set; }
        public string CModule { get; set; }
        public string CMethod { get; set; } // name view
        public string LangID { get; set; }
        public string MenuID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Customer { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
        public string TemplateID { get; set; }
        public DateTime Created { get; set; }
    }
    public class SysPageService : ServiceBase<SysPageEntity>
    {
        public SysPageService(IConfiguration config) : base(config, "SysPages")
        {

        }

        public SysPageService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public SysPageEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }

        public SysPageEntity GetItemByCtrlandAct(string ctrlName, string actName)
        {

            return CreateQuery().Find(o => o.CModule == ctrlName && o.CMethod == actName)?.SingleOrDefault();
        }
    }
}
