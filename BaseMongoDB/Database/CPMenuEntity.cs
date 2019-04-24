using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseMongoDB.Database
{
    public class CPMenuEntity : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ParentID { get; set; }
        public string Summary { get; set; }
        public string LangID { get; set; }
        public string Files { get; set; }
        public string Content { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class CPMenuService : ServiceBase<CPMenuEntity>
    {
        public CPMenuService(IConfiguration config) : base(config, "CPMenus")
        {

        }
        public List<CPMenuEntity> GetItemByType(string type, string langID)
        {
            var data = CreateQuery().Find(o => o.Type == type && o.LangID == langID)?.ToList();
            return data;
        }
        public List<CPMenuEntity> GetItemByType(string type)
        {
            var data = CreateQuery().Find(o => o.Type == type)?.ToList();
            return data;
        }
        public CPMenuEntity GetRoot(string type)
        {
            return CreateQuery().Find(o => o.Activity == true && string.IsNullOrEmpty(o.ParentID) && o.Type == type)?.SingleOrDefault();
        }
    }
}
