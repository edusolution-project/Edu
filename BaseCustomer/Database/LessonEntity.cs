using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class LessonEntity : CourseLessonEntity
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        //[JsonProperty("IsActive")]
        //public bool IsActive { get; set; }
        [JsonProperty("IsOnline")]
        public bool IsOnline { get; set; }
        [JsonProperty("IsHideAnswer")]
        public bool IsHideAnswer { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }

        [JsonProperty("GroupIDs")]
        public List<string> GroupIDs { get; set; }

        //[JsonProperty("LessonExtension")]
        //public List<LessonExtensionEntity> LessonExtension = new List<LessonExtensionEntity>();
        [JsonProperty("LessonExamID")]
        public List<String> LessonExamID { get; set; }

        public LessonEntity()
        {

        }

        public LessonEntity(CourseLessonEntity o, string suffix = "")
        {
            Media = o.Media;
            ChapterID = o.ChapterID;
            CreateUser = o.CreateUser;
            Code = o.Code;
            OriginID = o.ID;
            CourseID = o.CourseID;
            IsParentCourse = o.IsParentCourse;
            IsAdmin = o.IsAdmin;
            Timer = o.Timer;
            Point = o.Point;
            IsActive = o.IsActive;
            Title = o.Title + suffix;
            TemplateType = o.TemplateType;
            Order = o.Order;
            Created = DateTime.Now;
            Updated = DateTime.Now;
            Etype = o.Etype;
            Limit = o.Limit;
            Multiple = o.Multiple;
        }

    }

    public class LESSON_TEMPLATE
    {
        public const int LECTURE = 1, EXAM = 2;
    }

    public class LESSON_ETYPE
    {
        public const int PRACTICE = 0, WEEKLY = 1, CHECKPOINT = 2, EXPERIMENT = 3, INTERSHIP = 4, END = 5;
    }

    public class LessonService : ServiceBase<LessonEntity>
    {
        private IConfiguration config;
        private string v;

        public LessonService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonEntity>>
            {
                //ClassSubjectID_1_ChapterID_1_Order_1_ID_1
                //new CreateIndexModel<LessonEntity>(
                //    new IndexKeysDefinitionBuilder<LessonEntity>()
                //    .Ascending(t => t.ClassSubjectID)),
                //ChapterID_1_Order_1_ID_1
                new CreateIndexModel<LessonEntity>(
                    new IndexKeysDefinitionBuilder<LessonEntity>()
                    .Ascending(t=> t.ChapterID)
                    .Ascending(t=> t.Order)),
                //new CreateIndexModel<LessonEntity>(
                //    new IndexKeysDefinitionBuilder<LessonEntity>()
                //    .Ascending(t => t.ClassID)),

                new CreateIndexModel<LessonEntity>(
                    new IndexKeysDefinitionBuilder<LessonEntity>()
                    .Ascending(t => t.ClassID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate)),

                 new CreateIndexModel<LessonEntity>(
                    new IndexKeysDefinitionBuilder<LessonEntity>()
                    .Ascending(t => t.ClassSubjectID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate)),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public LessonService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public long CountClassLesson(string ClassID)
        {
            return Collection.CountDocumentsAsync(t => t.ClassID == ClassID).Result;
        }

        public long CountClassSubjectLesson(string ClassSubjectID)
        {
            return Collection.CountDocumentsAsync(t => t.ClassSubjectID == ClassSubjectID).Result;
        }

        public void UpdateLessonPoint(string ID, double point)
        {
            CreateQuery().UpdateOne(t => t.ID == ID, Builders<LessonEntity>.Update.Set(t => t.Point, point));
        }

        public IEnumerable<LessonEntity> GetChapterLesson(string ClassSubjectID, string ChapterID)
        {
            if (!string.IsNullOrEmpty(ClassSubjectID))
                return Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.ChapterID == ChapterID).SortBy(t => t.Order).ToEnumerable();
            else
                return Collection.Find(t => t.ChapterID == ChapterID).SortBy(t => t.Order).ToEnumerable();
        }

        public long CountChapterLesson(string ChapterID)
        {
            return Collection.CountDocumentsAsync(t => t.ChapterID == ChapterID).Result;
        }

        public IEnumerable<LessonEntity> GetClassSubjectLesson(string ClassSubjectID)
        {
            return Collection.Find(t => t.ClassSubjectID == ClassSubjectID).ToEnumerable();
        }

        public IEnumerable<LessonEntity> GetClassSubjectExams(string ClassSubjectID)
        {
            return Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.TemplateType == LESSON_TEMPLATE.EXAM).ToEnumerable();
        }

        public IEnumerable<LessonEntity> GetClassSubjectPractices(string ClassSubjectID)
        {
            return Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.IsPractice).ToEnumerable();
        }

        public long CountClassExam(string ClassID, DateTime? start = null, DateTime? end = null)
        {
            var validTime = new DateTime(1900, 1, 1);
            if (start == null && end == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.TemplateType == LESSON_TEMPLATE.EXAM);
            else if (start == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.TemplateType == LESSON_TEMPLATE.EXAM && (t.StartDate <= end));
            else if (end == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.TemplateType == LESSON_TEMPLATE.EXAM && (t.StartDate <= validTime || t.StartDate >= start));
            else
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.TemplateType == LESSON_TEMPLATE.EXAM && (t.StartDate <= validTime || (t.StartDate >= start && t.StartDate <= end)));
        }

        public IEnumerable<LessonEntity> GetClassActiveLesson(DateTime StartTime, DateTime EndTime, List<String> ClassIDs)
        {
            return Collection.Find(o => ClassIDs.Contains(o.ClassID) && o.StartDate <= EndTime && o.EndDate >= StartTime).ToEnumerable();
        }

        public IEnumerable<LessonEntity> GetClassSubjectActiveLesson(DateTime StartTime, DateTime EndTime, String ClassSubjectID)
        {
            return Collection.Find(o => o.ClassSubjectID == ClassSubjectID && o.StartDate <= EndTime && o.EndDate >= StartTime).ToEnumerable();
        }

        public IEnumerable<LessonEntity> GetIncomingSchedules(DateTime time, int period, string ClassID)
        {
            return Collection.Find(o => o.ClassID == ClassID && o.StartDate >= time && o.StartDate < time.AddMinutes(period)).ToEnumerable();
        }
    }
}
