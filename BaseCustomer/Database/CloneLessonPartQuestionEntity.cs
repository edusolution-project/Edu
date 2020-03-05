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
        public CloneLessonPartQuestionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CloneLessonPartQuestionEntity>>
            {
                //ClassID_1_ParentID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
                    .Ascending(t => t.ClassID).Ascending(t=> t.ParentID)),
                //ParentID_1
                new CreateIndexModel<CloneLessonPartQuestionEntity>(
                    new IndexKeysDefinitionBuilder<CloneLessonPartQuestionEntity>()
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
