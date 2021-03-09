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
    public class ClassSubjectEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SkillID")]
        public string SkillID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
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
        [JsonProperty("TypeClass")]
        public int TypeClass { get; set; }
    }

    public class ClassSubjectService : ServiceBase<ClassSubjectEntity>
    {
        public ClassSubjectService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<ClassSubjectEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ClassSubjectEntity>(
                    new IndexKeysDefinitionBuilder<ClassSubjectEntity>()
                    .Ascending(t => t.ClassID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }


        public ClassSubjectService(IConfiguration config, string dbName) : base(config, dbName)
        {
            
        }

        public List<ClassSubjectEntity> GetByClassID(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).ToList();
        }

        public List<string> GetIDsByClassID_Subject(string ClassID, string SubjectID)
        {
            return Collection.Find(t => t.ClassID == ClassID && t.SubjectID == SubjectID).Project(t => t.ID).ToList();
        }

        public List<string> GetCourseIdsByClassID(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).Project(t => t.CourseID).ToList();
        }

        public List<string> GetIdsByClassID(string ClassID)
        {
            return Collection.Find(t => t.ClassID == ClassID).Project(t => t.ID).ToList();
        }

        public List<ClassSubjectEntity> GetByTypeClass(String ClassID,Int32 typeClass)
        {
            return Collection.Find(x => x.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToList();
        }


        public Task RemoveByClass(string ClassID)
        {
            _ = Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            return Task.CompletedTask;
        }

        public Task RemoveManyClass(string[] ClassIDs)
        {
            _ = Collection.DeleteManyAsync(t => ClassIDs.Contains(t.ClassID));
            return Task.CompletedTask;
        }


        //Prevent this action
        //public Task UpdateCourseSkill(string CourseID, string SkillID)
        //{
        //    _ = Collection.UpdateManyAsync(t => t.CourseID == CourseID, Builders<ClassSubjectEntity>.Update.Set("SkillID", SkillID));
        //    return Task.CompletedTask;
        //}

        public long CountByCourseID(string CourseID)
        {
            return Collection.CountDocuments(t => t.CourseID == CourseID);
        }

        public List<ClassSubjectEntity> GetByCourseID(string CourseID)
        {
            return Collection.Find(t => t.CourseID == CourseID).ToList();
        }

        public ClassSubjectEntity GetClassSubjectExamByClassID(String ClassID)
        {
            return Collection.Find(x => x.ClassID == ClassID && x.TypeClass == CLASSSUBJECT_TYPE.EXAM).FirstOrDefault();
        }

        public IEnumerable<ClassSubjectEntity> GetByClassIds(List<string> classIds)
        {
            return Collection.Find(t => classIds.Contains(t.ClassID)).ToEnumerable();
        }

        public IEnumerable<ClassSubjectEntity> GetClassSubjectExamByClassIDs(List<String> ClassIDs)
        {
            return Collection.Find(x => ClassIDs.Contains(x.ClassID) && x.TypeClass == CLASSSUBJECT_TYPE.EXAM).ToEnumerable();
        }
    }

    public class CLASSSUBJECT_TYPE
    {
        public const int STANDARD = 0, //chính khóa
            EXTEND = 1, //bổ trợ
            EXAM = 2; //kiểm tra
    }

}
