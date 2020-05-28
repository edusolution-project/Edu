using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class FLessonPartAnswerEntity : LessonPartAnswerEntity
    {
    }
    public class FLessonPartAnswerService : ServiceBase<FLessonPartAnswerEntity>
    {
        public FLessonPartAnswerService(IConfiguration config) : base(config, "lessonpartanswer", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FLessonPartAnswerEntity>>
            {
                //CourseID_1
                new CreateIndexModel<FLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartAnswerEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<FLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartAnswerEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
