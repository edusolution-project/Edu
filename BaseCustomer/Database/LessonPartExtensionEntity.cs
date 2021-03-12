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
        [JsonProperty("Tags")]
        public List<String> Tags = new List<String>();
        //public String Tags { get; set; }
        [JsonProperty("TypePart")]
        public Int32 TypePart { get; set; }
    }

    public class LEVELPART //mức độ
    {
        public const int KNOW = 1, //Nhận biết
            UNDERSTANDING = 2, //Thông hiểu
            MANIPULATE = 3, //Vận dụng
            MANIPULATEHIGHLY = 4; // vận dụng cao
    }

    public class TYPE_PART
    {
        public const Int32 THEORY = 1,//Lý thuyết
            EXERCISE = 2;//bài tập
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
