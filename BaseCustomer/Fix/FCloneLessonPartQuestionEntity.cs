using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class FCloneLessonPartQuestionEntity : CloneLessonPartQuestionEntity
    {
    }

    public class FCloneLessonPartQuestionService : ServiceBase<FCloneLessonPartQuestionEntity>
    {
        public FCloneLessonPartQuestionService(IConfiguration config) : base(config, "clonelessonpartquestion", config.GetSection("dbName:EdusoBAK").Value)
        {
            var indexs = new List<CreateIndexModel<FCloneLessonPartQuestionEntity>>
            {
                //ClassID_1
                new CreateIndexModel<FCloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartQuestionEntity>()
                    .Ascending(t => t.ClassID)),
                //ClassSubjectID_1
                new CreateIndexModel<FCloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartQuestionEntity>()
                    .Ascending(t => t.ClassSubjectID)),
                //ParentID_1
                new CreateIndexModel<FCloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<FCloneLessonPartQuestionEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task RemoveManyAsync(List<string> Ids)
        {
            _ = Collection.DeleteManyAsync(t => Ids.Contains(t.ID));
        }
    }

}
