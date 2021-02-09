using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartExtensionEntity : CloneLessonPartEntity
    {

    }

    public class CloneLessonPartExtensionService : ServiceBase<CloneLessonPartExtensionEntity>
    {
        public CloneLessonPartExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartExtensionEntity>>
            {
                new CreateIndexModel<CloneLessonPartExtensionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartExtensionEntity>()
                    .Ascending(t=> t.ID)),
                new CreateIndexModel<CloneLessonPartExtensionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartExtensionEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<CloneLessonPartExtensionEntity> GetItemsByLessonID(string ParentID)
        {
            return CreateQuery().Find(x => x.ParentID == ParentID).ToEnumerable();
        }

        public List<String> GetIDsByLessonID(string ParentID)
        {
            return CreateQuery().Find(x => x.ParentID == ParentID).Project(x=>x.ID).ToList();
        }
    }
}
