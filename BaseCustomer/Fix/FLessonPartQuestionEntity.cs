using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class FLessonPartQuestionEntity : LessonPartQuestionEntity
    {
    }
    public class FLessonPartQuestionService : ServiceBase<FLessonPartQuestionEntity>
    {
        public FLessonPartQuestionService(IConfiguration config) : base(config, "lessonpartquestion", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FLessonPartQuestionEntity>>
            {
                //CourseID_1
                new CreateIndexModel<FLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartQuestionEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<FLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartQuestionEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
