using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class LessonPartExtensionEntity : LessonPartEntity
    {
        //public LessonPartExtensionEntity(LessonPartEntity o)
        //{

        //}
        [JsonProperty("LevelPart")]
        public Int32 LevelPart { get; set; }
        [JsonProperty("ExamQuestionArchiveID")]
        public String ExamQuestionArchiveID { get; set; }
        [JsonProperty("GradeID")]
        public String GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public String SubjectID { get; set; }
    }

    public class LEVELPART
    {
        public const int EASAY = 1, //DỄ
            NORMAL = 2, //TRUNG BÌNH
            HARD = 3; //KHÓ
    }

    public class LessonPartExtensionService : ServiceBase<LessonPartExtensionEntity>
    {
        public LessonPartExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonPartExtensionEntity>>
            {
                new CreateIndexModel<LessonPartExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartExtensionEntity>()
                    .Ascending(t=> t.ID)),
                new CreateIndexModel<LessonPartExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartExtensionEntity>()
                    .Ascending(t=> t.ParentID)),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<LessonPartExtensionEntity> GetItemsByExamQuestionArchiveID(string examQuestionArchiveID)
        {
            return CreateQuery().Find(x => x.ExamQuestionArchiveID == examQuestionArchiveID).ToEnumerable();
        }

        public IEnumerable<LessonPartExtensionEntity> GetItemsByLessonID(string ParentID)
        {
            return CreateQuery().Find(x => x.ParentID == ParentID).ToEnumerable();
        }
    }
}
