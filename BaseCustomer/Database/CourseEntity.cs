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
        public string Outline { get; set; }
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
        [JsonProperty("TargetCenters")]
        public List<string> TargetCenters { get; set; }
        [JsonProperty("StudentTargetCenters")]
        public List<string> StudentTargetCenters { get; set; }
        [JsonProperty("PublishedVer")]
        public DateTime PublishedVer { get; set; } // root to current
        [JsonProperty("LastSync")]
        public DateTime LastSync { get; set; } // current to clone
        //[JsonProperty("IsPublic")]
        //public Boolean IsPublic { get; set; }
        //[JsonProperty("PublicWStudent")]
        //public Boolean PublicWStudent { get; set; }
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
                    .Ascending(t => t.Center)
                    .Ascending(t => t.TeacherID)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t => t.GradeID)
                    .Ascending(t => t.IsActive)
                    ),
                //Center_1_GradeID_1_IsActive_1
                new CreateIndexModel<CourseEntity>(
                    new IndexKeysDefinitionBuilder<CourseEntity>()
                    .Ascending(t => t.Center)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t => t.GradeID)
                    .Ascending(t => t.IsActive)
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

        public CourseEntity GetCopiedItemInCenter(string ID, string CenterID)
        {
            return CreateQuery().Find(t => t.Center == CenterID && t.OriginID == ID).SortByDescending(t => t.ID).FirstOrDefault();
        }

        public void ShareToCenter(string ID, string TargetID)
        {
            CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<CourseEntity>()
                .AddToSet(t => t.TargetCenters, TargetID));
        }

        public IEnumerable<CourseEntity> GetItemBySubjectID_GradeID(String SubjectID, String GradeID, String CenterID)
        {
            return CreateQuery().Find(x => x.GradeID.Equals(GradeID) && x.SubjectID.Equals(SubjectID) && x.Center.Equals(CenterID)).ToEnumerable();
        }
    }
}
