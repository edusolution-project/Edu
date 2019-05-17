using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using System;

namespace BaseMongoDB.Database
{
    public class ModProgramEntity : EntityBase
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
    public class ModProgramService : ServiceBase<ModProgramEntity>
    {
        public ModProgramService(IConfiguration config) : base(config, "ModPrograms")
        {

        }
        public ModProgramService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
