using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartAnswerEntity : LessonPartAnswerEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
    }
    public class CloneLessonPartAnswerService : ServiceBase<CloneLessonPartAnswerEntity>
    {
        public CloneLessonPartAnswerService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartAnswerEntity>>
            {
                //CourseID_1
                new CreateIndexModel<CloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartAnswerEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<CloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartAnswerEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
