using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartAnswerEntity : LessonPartAnswerEntity
    {

        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
    }
    public class CloneLessonPartAnswerService : ServiceBase<CloneLessonPartAnswerEntity>
    {
        private IConfiguration config;
        private string dbName;

        public CloneLessonPartAnswerService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartAnswerEntity>>
            {
                //CourseID_1
                new CreateIndexModel<CloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartAnswerEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<CloneLessonPartAnswerEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartAnswerEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public CloneLessonPartAnswerService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public async Task RemoveByParentAsync(string ParentID)
        {
            await Collection.DeleteManyAsync(t => t.ParentID == ParentID);
        }

        public async Task RemoveManyAsync(List<string> IDs)
        {
            await Collection.DeleteManyAsync(t => IDs.Contains(t.ID));
        }

        public IEnumerable<CloneLessonPartAnswerEntity> GetByQuestionID(string ID)
        {
            return CreateQuery().Find(o => o.ParentID == ID).ToEnumerable();
        }
    }
}
