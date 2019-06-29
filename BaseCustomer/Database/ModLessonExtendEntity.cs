using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class ModLessonExtendEntity : EntityBase
    {
        [JsonProperty("LessonPartID")]
        public string LessonPartID { get; set; }
        [JsonProperty("NameOriginal")]
        public string NameOriginal { get; set; }
        [JsonProperty("File")]
        public string File { get; set; }
        [JsonProperty("OriginalFile")]
        public string OriginalFile { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
    public class ModLessonExtendService : ServiceBase<ModLessonExtendEntity>
    {
        public ModLessonExtendService(IConfiguration config) : base(config, "ModLessonExtends", "VES")
        {

        }
    }
}
