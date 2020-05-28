using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class FCloneLessonPartAnswerEntity : CloneLessonPartAnswerEntity
    {
    }
    public class FCloneLessonPartAnswerService: ServiceBase<FCloneLessonPartAnswerEntity>
    {
        public FCloneLessonPartAnswerService(IConfiguration config) : base(config, "clonelessonpartanswer", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FCloneLessonPartAnswerEntity>>
            {
                //CourseID_1
                new CreateIndexModel<FCloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartAnswerEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<FCloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartAnswerEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task RemoveByParentAsync(string ParentID)
        {
            await Collection.DeleteManyAsync(t => t.ParentID == ParentID);
        }

        public async Task RemoveManyAsync(List<string> IDs)
        {
            await Collection.DeleteManyAsync(t => IDs.Contains(t.ID));
        }
    }
}
