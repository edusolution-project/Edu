﻿using Core_v2.Repositories;
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
        [JsonProperty("Students")]
        public List<string> Students { get; set; } = new List<string>();
        [JsonProperty("Members")]
        public List<ClassMemberEntity> Members { get; set; }
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
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsGroup")]
        public bool? IsGroup { get; set; } = false;
        [JsonProperty("Subjects")]
        public List<string> Subjects { get; set; }
        [JsonProperty("Skills")]
        public List<string> Skills { get; set; }

        //Multiple
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
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
        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
        [JsonProperty("TotalExams")]
        public long TotalExams { get; set; }
        [JsonProperty("TotalPractices")]
        public long TotalPractices { get; set; }
        [JsonProperty("Center")]
        public string Center { get; set; }
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
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
                    .Ascending(t=> t.GradeID)),
                new CreateIndexModel<ClassEntity>(
                    new IndexKeysDefinitionBuilder<ClassEntity>()
                    .Text(t=> t.Name))
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

        public long AddMemberToClass(string ID, ClassMemberType member)
        {
            return CreateQuery().UpdateManyAsync(t => t.ID.Equals(ID),
                Builders<ClassEntity>.Update.AddToSet("Members", member)).Result.ModifiedCount;
        }

        public long AddSubjectToClass(string ID, string subjectID)
        {
            return CreateQuery().UpdateManyAsync(t => t.ID.Equals(ID),
                Builders<ClassEntity>.Update.AddToSet("Subjects", subjectID)).Result.ModifiedCount;
        }

        public IEnumerable<string> GetMultipleClassName(List<string> IDs, string CenterID = "")
        {
            if (string.IsNullOrEmpty(CenterID))
                return Collection.Find(t => IDs.Contains(t.ID)).Project(t => t.Name).ToEnumerable();
            return Collection.Find(t => IDs.Contains(t.ID) && t.Center == CenterID).Project(t => t.Name).ToEnumerable();
        }

        public IEnumerable<ClassEntity> GetItemsByIDs(List<string> ClassIDs)
        {
            return Collection.Find(t => ClassIDs.Contains(t.ID)).ToEnumerable();
        }

        public IEnumerable<ClassEntity> GetItemsByIDs(List<string> ClassIDs, string CenterID)
        {
            return Collection.Find(t => ClassIDs.Contains(t.ID) && t.Center == CenterID).ToEnumerable();
        }

        public IEnumerable<string> GetTeacherClassList(string userID)
        {
            return Collection.Find(t => t.Members.Any(o => o.TeacherID == userID)).Project(t => t.ID).ToEnumerable();
        }

        public IEnumerable<ClassEntity> GetActiveClass(DateTime time, string Center = null)
        {
            if (string.IsNullOrEmpty(Center))
                return Collection.Find(t => t.StartDate < time && t.EndDate > time).ToEnumerable();
            else
                return Collection.Find(t => t.Center == Center && t.StartDate < time && t.EndDate > time).ToEnumerable();
        }
    }
}
