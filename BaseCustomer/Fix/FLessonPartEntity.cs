using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class FLessonPartEntity : LessonPartEntity
    {
    }

    public class FLessonPartService : ServiceBase<FLessonPartEntity>
    {
        public FLessonPartService(IConfiguration config) : base(config, "lessonpart", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FLessonPartEntity>>
            {
                //CourseID_1
                new CreateIndexModel<FLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<FLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<FLessonPartEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
