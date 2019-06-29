using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class LessonPartAnswerEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
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
    public class LessonPartAnswerService : ServiceBase<LessonPartAnswerEntity>
    {
        public LessonPartAnswerService(IConfiguration config) : base(config)
        {

        }
    }
}
