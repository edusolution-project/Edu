using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartAnswerExtensionEntity : CloneLessonPartAnswerEntity
    {
    }

    public class CloneLessonPartAnswerExtensionService : ServiceBase<CloneLessonPartAnswerExtensionEntity>
    {
        public CloneLessonPartAnswerExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartAnswerExtensionEntity>>
            {
                new CreateIndexModel<CloneLessonPartAnswerExtensionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartAnswerExtensionEntity>()
                    .Ascending(t=> t.ID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<CloneLessonPartAnswerExtensionEntity> GetByQuestionID(string ID)
        {
            return CreateQuery().Find(o => o.ParentID == ID).ToEnumerable();
        }

        public IEnumerable<CloneLessonPartAnswerExtensionEntity> GetItemsByQuestionIDs(List<string> questionIDs)
        {
            return CreateQuery().Find(o => questionIDs.Contains(o.ParentID)).ToEnumerable();
        }

        public List<CloneLessonPartAnswerEntity> ConvertToCloneLessonPartAns(List<CloneLessonPartAnswerExtensionEntity> datas)
        {
            var newlist = new List<CloneLessonPartAnswerEntity>();
            foreach(var item in datas)
            {
                newlist.Add(item);
            }
            return newlist;
        }
    }
}
