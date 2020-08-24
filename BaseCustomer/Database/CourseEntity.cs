using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class CourseEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("SkillID")]
        public string SkillID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("IsUsed")]
        public bool IsUsed { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Outline")]
        public string Outline { get; set; }             // Đề cương môn học
        [JsonProperty("LearningOutcomes")]
        public string LearningOutcomes { get; set; }  // Mục tiêu môn học
        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
        [JsonProperty("TotalExams")]
        public long TotalExams { get; set; }
        [JsonProperty("TotalPractices")]
        public long TotalPractices { get; set; }
        [JsonProperty("Center")]
        public string Center { get; set; }
    }

    public class CourseService : ServiceBase<CourseEntity>
    {

        public CourseService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CourseEntity>>
            {
                //TeacherID_1_SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Ascending(t=> t.TeacherID)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID)
                    .Ascending(t=>t.IsActive)
                    ),
                //SubjectID_1_GradeID_1_IsActive_1
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID)
                    ),
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Text(t=> t.Name)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task IncreaseLessonCounter(string ID, long lesInc, long examInc, long pracInc)
        {
            await CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<CourseEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
        }
    }
}
