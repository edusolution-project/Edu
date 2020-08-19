using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }

    }
    public class LessonPartQuestionService : ServiceBase<LessonPartQuestionEntity>
    {
        public LessonPartQuestionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonPartQuestionEntity>>
            {
                //CourseID_1
                new CreateIndexModel<LessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartQuestionEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<LessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartQuestionEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
