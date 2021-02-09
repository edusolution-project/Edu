using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class LessonPartAnswerExtensionEntity : LessonPartAnswerEntity
    {
        public LessonPartAnswerExtensionEntity()
        {

        }
        public LessonPartAnswerExtensionEntity(LessonPartAnswerEntity o)
        {
            Content = o.Content;
            IsCorrect = o.IsCorrect;
            Media = o.Media;
            Medias = o.Medias;
            Order = o.Order;
            CourseID = o.CourseID;
        }
    }

    public class LessonPartAnswerExtensionService : ServiceBase<LessonPartAnswerExtensionEntity>
    {
        public LessonPartAnswerExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonPartAnswerExtensionEntity>>
            {
                //CourseID_1
                new CreateIndexModel<LessonPartAnswerExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartAnswerExtensionEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<LessonPartAnswerExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartAnswerExtensionEntity>()
                    .Ascending(t=> t.ParentID)),

                new CreateIndexModel<LessonPartAnswerExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartAnswerExtensionEntity>()
                    .Ascending(t=> t.ID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<LessonPartAnswerExtensionEntity> GetItemByParentID(string iD)
        {
            return CreateQuery().Find(x => x.ParentID == iD).ToEnumerable();
        }
    }
}
