using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;

namespace BaseMongoDB.Database
{
    public class CPLangEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
    }
    public class CPLangService : ServiceBase<CPLangEntity>
    {
        public CPLangService(IConfiguration config) : base(config, "CPLangs")
        {

        }
        public CPLangService(IConfiguration config,string tableName) : base(config, tableName)
        {

        }
        public CPLangEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
