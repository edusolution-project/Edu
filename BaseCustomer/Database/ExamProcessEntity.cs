using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ExamProcessEntity : EntityBase
    {
        [JsonProperty("ExamQuestionArchiveID")] //ma kho de
        public String ExamQuestionArchiveID { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("StartTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("EndTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty("EasyQuestion")]
        public Int32 EasyQuestion { get; set; }
        [JsonProperty("NormalQuestion")]
        public Int32 NormalQuestion { get; set; }
        [JsonProperty("HardQuestion")]
        public Int32 HardQuestion { get; set; }
        //[JsonProperty("CodeExam")]
        //public String CodeExam { get; set; }
        [JsonProperty("Timer")] // thời gian làm bài
        public Int32 Timer { get; set; }
        [JsonProperty("Limit")]
        public Int32 Limit { get; set; }
        [JsonProperty("Etype")]
        public Int32 Etype { get; set; }
        [JsonProperty("Multiple")]
        public Int32 Multiple { get; set; }
        [JsonProperty("TargetClasses")]
        public List<String> TargetClasses { get; set; }
        [JsonProperty("TotalExam")]
        public Int32 TotalExam { get; set; } //so de muon tao
        [JsonProperty("LessonID")]
        public String LessonID { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
    }

    public class ExamProcessService : ServiceBase<ExamProcessEntity>
    {
        public ExamProcessService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ExamProcessEntity>>
            {
                //TeacherID_1_SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<ExamProcessEntity>(
                    new IndexKeysDefinitionBuilder<ExamProcessEntity>()
                    .Ascending(t => t.ID)
                    ),
                //new CreateIndexModel<ExamProcessEntity>(
                //    new IndexKeysDefinitionBuilder<ExamProcessEntity>()
                //    .Ascending(t => t.ID)
                //    ),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
