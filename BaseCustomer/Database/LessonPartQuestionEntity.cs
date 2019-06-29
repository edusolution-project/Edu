using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class LessonPartQuestionEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }

    }
    public class LessonPartQuestionService : ServiceBase<LessonPartQuestionEntity>
    {
        public LessonPartQuestionService(IConfiguration config) : base(config)
        {

        }
    }
}
