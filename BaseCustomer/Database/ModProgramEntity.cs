using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class ModProgramEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Grades")]
        public List<string> Grades { get; set; }
        [JsonProperty("Subjects")]
        public List<string> Subjects { get; set; }


    }
    public class ModProgramService : ServiceBase<ModProgramEntity>
    {
        public ModProgramService(IConfiguration config) : base(config, "ModPrograms", "VES")
        {

        }
        public ModProgramService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public List<ModProgramEntity> FindBySubject(string id)
        {
            var filter = Builders<ModProgramEntity>.Filter.AnyEq("Subjects", id);
            return CreateQuery().Find(filter).ToList();
        }

        public List<ModProgramEntity> FindByGrade(string id)
        {
            var filter = Builders<ModProgramEntity>.Filter.AnyEq("Grades", id);
            return CreateQuery().Find(filter).ToList();
        }


        public object GetItemByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }
}
