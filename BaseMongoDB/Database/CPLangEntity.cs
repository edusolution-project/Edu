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
        public bool Activity { get; set; }
    }
    public class CPLangService : ServiceBase<CPLangEntity>
    {
        public CPLangService(IConfiguration config) : base(config, "CPLangs")
        {

        }
        public CPLangEntity GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
