using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class FCloneLessonPartEntity : CloneLessonPartEntity
    {
    }

    public class FCloneLessonPartService : ServiceBase<FCloneLessonPartEntity>
    {
        public FCloneLessonPartService(IConfiguration config) : base(config, "clonelessonpart", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FCloneLessonPartEntity>>
            {
                //ClassID_1_ParentID_1
                new CreateIndexModel<FCloneLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartEntity>()
                    .Ascending(t => t.ClassID).Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<FCloneLessonPartEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
