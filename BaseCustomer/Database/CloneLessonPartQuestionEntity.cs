using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartQuestionEntity : LessonPartQuestionEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
    }
    public class CloneLessonPartQuestionService : ServiceBase<CloneLessonPartQuestionEntity>
    {
        private IConfiguration config;
        private string dbName;

        public CloneLessonPartQuestionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartQuestionEntity>>
            {
                //ClassID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
                    .Ascending(t => t.ClassID)),
                //ClassSubjectID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
                    .Ascending(t => t.ClassSubjectID)),
                //LessonID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
                    .Ascending(t => t.LessonID)),
                //ParentID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public CloneLessonPartQuestionService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public async Task RemoveManyAsync(List<string> Ids)
        {
            await CreateQuery().DeleteManyAsync(t => Ids.Contains(t.ID));
        }

        public IEnumerable<CloneLessonPartQuestionEntity> GetByPartID(string PartID)
        {
            return CreateQuery().Find(o => o.ParentID == PartID).ToEnumerable();
        }

        public long CountByLessonID(string LessonID)
        {
            return CreateQuery().CountDocuments(t => t.LessonID == LessonID);
        }
    }

}
