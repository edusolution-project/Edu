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
        [JsonProperty("TeacherID")]//CreatorID
        public string TeacherID { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        //[JsonProperty("CreatorName")]//CreatorID
        //public string CreatorName { get; set; }

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
        [JsonProperty("ClassMechanism")]

        public int ClassMechanism { get; set; } //cơ chế lớp
    }

    public class ClassService : ServiceBase<ClassEntity>
    {
        public ClassService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ClassEntity>>
            {
                new CreateIndexModel<ClassEntity>(
                    new IndexKeysDefinitionBuilder<ClassEntity>()
                    .Text(t=> t.Name)),
                //Center_1
                new CreateIndexModel<ClassEntity>(
                    new IndexKeysDefinitionBuilder<ClassEntity>()
                    .Ascending(t=> t.Center))

            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public ClassService(IConfiguration config, string dbName) : base(config, dbName)
        {

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

        public IEnumerable<string> GetMultipleClassName(List<string> IDs, string StudentID = "", string CenterID = "")
        {
            if (!string.IsNullOrEmpty(StudentID) && GetClassByMechanism(CLASS_MECHANISM.PERSONAL, StudentID) != null)
            {
                var index = IDs.IndexOf(GetClassByMechanism(CLASS_MECHANISM.PERSONAL, StudentID).ID);
                if (index >= 0)
                    IDs.RemoveAt(index);
            }
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

        public IEnumerable<string> GetTeacherClassListID(string userID)
        {
            return Collection.Find(t => t.Members.Any(o => o.TeacherID == userID)).Project(t => t.ID).ToEnumerable();
        }

        public List<ClassEntity> GetTeacherClassList(String UserID)
        {
            return Collection.Find(t => t.Members.Any(o => o.TeacherID == UserID)).ToList();
        }

        public IEnumerable<ClassEntity> GetActiveClass(DateTime time, string Center = null)
        {
            if (string.IsNullOrEmpty(Center))
                return Collection.Find(t => t.StartDate < time && t.EndDate > time && t.IsActive && t.ClassMechanism != CLASS_MECHANISM.PERSONAL && t.IsActive).ToEnumerable();
            else
                return Collection.Find(t => t.Center == Center && t.StartDate < time && t.EndDate > time && t.IsActive && t.ClassMechanism != CLASS_MECHANISM.PERSONAL && t.IsActive).ToEnumerable();
        }

        public IEnumerable<ClassEntity> GetActiveClass4Report(DateTime firstTime, DateTime lastTime, String centerID)
        {
            return Collection.Find(t => t.Center == centerID && t.StartDate <= lastTime && t.EndDate >= firstTime && t.ClassMechanism != CLASS_MECHANISM.PERSONAL && t.IsActive).ToEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ClassMechanism"></param>
        /// <param name="StudentID"></param>
        /// <returns></returns>
        public ClassEntity GetClassByMechanism(int ClassMechanism, string StudentID)
        {
            return Collection.Find(c => c.ClassMechanism == ClassMechanism && c.TeacherID == StudentID).FirstOrDefault();
        }
    }

    public class CLASS_MECHANISM //cơ chế lớp
    {
        public const int CLOSE = 0, //Lop dong
            OPEN = 1, //Lop mo
            PERSONAL = 2;//Lop ca nhan
    }

    public class CLASS_LEVEL //Khoi
    {
        public const int
            UNSET = 0,
            K1 = 1,
            K2 = 2,
            K3 = 3,
            K4 = 4,
            K5 = 5,
            K6 = 6,
            K7 = 7,
            K8 = 8,
            K9 = 9,
            K10 = 10,
            K11 = 11,
            K12 = 12;
    }
}
