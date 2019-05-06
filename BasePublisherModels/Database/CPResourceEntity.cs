using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BasePublisherModels.Database
{
    public class CPResourceEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string LangID { get; set; }
    }
    public class CPResourceService : ServiceBase<CPResourceEntity>
    {
        public CPResourceService(IConfiguration config) : base(config, "CPResources")
        {

        }

        public CPResourceService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public CPResourceEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
        public List<CPResourceEntity> GetByLangID(string LangID)
        {
            return CreateQuery().Find(o => o.LangID == LangID)?.ToList();
        }
    }
}
