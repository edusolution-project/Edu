using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ExamQuestionArchiveEntity : EntityBase //kho de
    {
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("GradeID")] //cap do
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")] //chuong trinh
        public String SubjectID { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("IsShare")]
        public Boolean IsShare { get; set; }
        [JsonProperty("IsActive")]
        public Boolean IsActive { get; set; } //mac dinh se la false
        [JsonProperty("CenterID")]
        public String CenterID { get; set; }
        [JsonProperty("Description")]
        public String Description { get; set; }
        [JsonProperty("TargetClasses")]
        public List<String> TargetClasses { get; set; }
        [JsonProperty("TotalTime")]
        public Int32 TotalTime { get; set; }
        [JsonProperty("MainSubject")]
        public String MainSubject { get; set; }
        //[JsonProperty("QuestionEasy")]
        //public Int32 QuestionEasy { get; set; } //so cau de
        //[JsonProperty("QuestionNormal")]
        //public Int32 QuestionNormal { get; set; } // so cau trung binh
        //[JsonProperty("QuestionHard")]
        //public Int32 QuestionHard { get; set; } // so cau kho
    }

    public class ExamQuestionArchiveService : ServiceBase<ExamQuestionArchiveEntity>
    {
        public ExamQuestionArchiveService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ExamQuestionArchiveEntity>>
            {
                //TeacherID_1_SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<ExamQuestionArchiveEntity>(
                    new IndexKeysDefinitionBuilder<ExamQuestionArchiveEntity>()
                    .Ascending(t => t.CenterID)
                    .Ascending(t => t.CreateUser)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t => t.GradeID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<ExamQuestionArchiveEntity> GetItemByIDs(List<string> IDs)
        {
            return CreateQuery().Find(x => IDs.Contains(x.ID)).ToEnumerable();
        }
    }
}
