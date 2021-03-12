using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartExtensionEntity : CloneLessonPartEntity
    {
        [JsonProperty("LessonExamID")]
        public String LessonExamID { get; set; }
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

        public IEnumerable<CloneLessonPartExtensionEntity> GetItemsByLessonID(string ParentID,String classSubjectID)
        {
            return CreateQuery().Find(x => x.ParentID == ParentID && x.ClassSubjectID == classSubjectID).ToEnumerable();
        }

        public List<CloneLessonPartEntity> GetitemsByLessonExamID(string LessonExamID)
        {
            var list = CreateQuery().Find(x => x.LessonExamID == LessonExamID).ToList();
            var newlist = new List<CloneLessonPartEntity>();
            foreach(var item in list)
            {
                newlist.Add(item as CloneLessonPartEntity);
            }
            return newlist;
        }

        public IEnumerable<CloneLessonPartExtensionEntity> GetItemsByLessonExamID(List<string> lessonExamIDs)
        {
            return CreateQuery().Find(x => lessonExamIDs.Contains(x.LessonExamID)).ToEnumerable();
        }

        public long CountByLessonID(string lessonExamID)
        {
            return CreateQuery().CountDocuments(t => t.LessonExamID == lessonExamID);
        }
    }
}
