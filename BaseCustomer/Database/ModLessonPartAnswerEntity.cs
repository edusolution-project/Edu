using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class ModLessonPartAnswerEntity : EntityBase
    {
        [JsonProperty("ParentID")]
        public string ParentID { get; set; } // chính là lessonPartID
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("IsCorrect")]
        public bool IsCorrect { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }

    }
    public class ModLessonPartAnswerService : ServiceBase<ModLessonPartAnswerEntity>
    {
        public ModLessonPartAnswerService(IConfiguration config) : base(config, "ModLessonPartAnswers")
        {

        }
        public ModLessonPartAnswerService(IConfiguration config, string tableName) : base(config, tableName)
        {

        }
    }
}
