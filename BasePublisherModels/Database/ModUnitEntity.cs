using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;

namespace BasePublisherModels.Database
{
    public class ModUnitEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public int Order { get; set; }
    }
    public class ModUnitService : ServiceBase<ModUnitEntity>
    {
        public ModUnitService(IConfiguration config) : base(config, "ModUnits")
        {

        }
        public ModUnitService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
