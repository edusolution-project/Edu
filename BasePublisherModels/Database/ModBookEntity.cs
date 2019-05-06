using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;

namespace BasePublisherModels.Database
{
    public class ModBookEntity : EntityBase
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
    public class ModBookService : ServiceBase<ModBookEntity>
    {
        public ModBookService(IConfiguration config) : base(config, "ModBooks")
        {

        }
        public ModBookService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }

        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
