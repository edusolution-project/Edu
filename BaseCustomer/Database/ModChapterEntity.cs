using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class ModChapterEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("ParentType")]
        public int ParentType { get; set; }
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
    }
    public class ModChapterService : ServiceBase<ModChapterEntity>
    {
        public ModChapterService(IConfiguration config) : base(config, "ModChapters", config.GetSection("dbName:Publisher").Value)
        {

        }
        public object GetByCode(string code)
        {
            return CreateQuery().Find(o => o.Code == code)?.SingleOrDefault();
        }
    }

    public static class PARENT_TYPE_CODE
    {
        public const int COURSE = 1, CHAPTER = 2;
    }
}
