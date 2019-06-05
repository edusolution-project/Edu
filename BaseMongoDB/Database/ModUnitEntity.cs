using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace BaseMongoDB.Database
{
    public class ModUnitEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        public bool Activity { get; set; }
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
    }
}
