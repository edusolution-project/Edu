using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class ClassEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("Students")]
        public List<string> Students { get; set; } = new List<string>();
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("Syllabus")]
        public string Syllabus { get; set; }
        [JsonProperty("Modules")]
        public string Modules { get; set; }
        [JsonProperty("References")]
        public string References { get; set; }
        [JsonProperty("LearningOutcomes")]
        public string LearningOutcomes { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsGroup")]
        public bool? IsGroup { get; set; } = false;
    }

    public class ClassService : ServiceBase<ClassEntity>
    {
        public ClassService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ClassEntity>>
            {
                //SubjectID_1_GradeID_1_TeacherID_1
                new CreateIndexModel<ClassEntity>(
                    new IndexKeysDefinitionBuilder<ClassEntity>()
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID)
                    .Ascending(t=> t.TeacherID)),
                //TeacherID_1SubjectID_1_GradeID_1_
                new CreateIndexModel<ClassEntity>(
                    new IndexKeysDefinitionBuilder<ClassEntity>()
                    .Ascending(t=> t.TeacherID)
                    .Ascending(t => t.SubjectID)
                    .Ascending(t=> t.GradeID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public long RemoveStudent(string ID, string studentID)
        {
            return CreateQuery().UpdateManyAsync(t => t.ID.Equals(ID),
                Builders<ClassEntity>.Update.Pull("Students", studentID)).Result.ModifiedCount;
        }

        public long AddStudentToClass(string ID, string studentID)
        {
            return CreateQuery().UpdateManyAsync(t => t.ID.Equals(ID),
                Builders<ClassEntity>.Update.AddToSet("Students", studentID)).Result.ModifiedCount;
        }
    }
}
