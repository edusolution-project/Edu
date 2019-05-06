using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePublisherModels.Database
{
    public class ModSubjectEntity : EntityBase
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
    public class ModSubjectService : ServiceBase<ModSubjectEntity>
    {
        public ModSubjectService(IConfiguration config) : base(config, "ModSubject")
        {
        }
        public ModSubjectService(IConfiguration config,string tableName) : base(config, tableName)
        {
        }
    }
}
