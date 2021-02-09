using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartQuestionExtensionEntity : CloneLessonPartQuestionEntity
    {
    }

    public class CloneLessonPartQuestionExtensionService : ServiceBase<CloneLessonPartQuestionExtensionEntity>
    {
        public CloneLessonPartQuestionExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartQuestionExtensionEntity>>
            {
                new CreateIndexModel<CloneLessonPartQuestionExtensionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionExtensionEntity>()
                    .Ascending(t=> t.ID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public long CountByLessonID(string lessonID)
        {
            return CreateQuery().CountDocuments(t => t.LessonID == lessonID);
        }

        public IEnumerable<CloneLessonPartQuestionExtensionEntity> GetItemsByParentIDs(List<String> ParentIDs)
        {
            return CreateQuery().Find(x => ParentIDs.Contains(x.ParentID)).ToEnumerable();
        }
    }
}
