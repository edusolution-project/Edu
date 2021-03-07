using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class LessonScheduleEntity : EntityBase
    {
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
        [JsonProperty("Type")]
        public int? Type { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("IsOnline")]
        public bool IsOnline { get; set; }
        [JsonProperty("IsHideAnswer")]
        public bool IsHideAnswer { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }

    }

    public class SCHEDULE_TYPE
    {
        public const int LECTURE = LESSON_TEMPLATE.LECTURE,
            EXAM = LESSON_TEMPLATE.EXAM,
            WEBINAR = 3;
    }

    public class LessonScheduleService : ServiceBase<LessonScheduleEntity>
    {
        public LessonScheduleService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonScheduleEntity>>
            {
                //LessonID_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t => t.LessonID)),
                //ClassID_1_StartDate_1_EndDate_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t=> t.ClassID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate)),
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t=> t.ClassSubjectID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public LessonScheduleService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public IEnumerable<LessonScheduleEntity> GetIncomingSchedules(DateTime time, int period, string ClassID)
        {
            return Collection.Find(o => o.ClassID == ClassID && o.StartDate >= time && o.StartDate < time.AddMinutes(period)).ToEnumerable();
        }

        //public LessonScheduleEntity GetItemByLessonID_ClassSubjectID(string lessonid, string classsubjectid)
        //{
        //    if (string.IsNullOrEmpty(lessonid) || lessonid == "0" || string.IsNullOrEmpty(classsubjectid)) return null;
        //    return Collection.Find(o => o.LessonID == lessonid && o.ClassSubjectID == classsubjectid)?.SingleOrDefault();
        //}

        public LessonScheduleEntity GetItemByLessonID(string lessonid)
        {
            if (string.IsNullOrEmpty(lessonid) || lessonid == "0") return null;
            return Collection.Find(o => o.LessonID == lessonid)?.SingleOrDefault();
        }

        //public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        //{
        //    await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<LessonScheduleEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        //}

        public long CountClassExam(string ClassID, DateTime? start = null, DateTime? end = null)
        {
            var validTime = new DateTime(1900, 1, 1);
            if (start == null && end == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM);
            else if (start == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= end));
            else if (end == null)
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || t.StartDate >= start));
            else
                return Collection.CountDocuments(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || (t.StartDate >= start && t.StartDate <= end)));
        }

        public List<LessonScheduleEntity> GetClassExam(string ClassID, DateTime? start = null, DateTime? end = null)
        {
            var validTime = new DateTime(1900, 1, 1);
            IFindFluent<LessonScheduleEntity, LessonScheduleEntity> data;
            if (start == null && end == null)
                data = Collection.Find(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM);
            else if (start == null)
                data = Collection.Find(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= end));
            else if (end == null)
                data = Collection.Find(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || t.StartDate >= start));
            else
                data = Collection.Find(t => t.ClassID == ClassID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || (t.StartDate >= start && t.StartDate <= end)));
            return data.SortByDescending(t => t.StartDate).ToList();
        }

        public long CountClassSubjectExam(List<string> ClassSubjectIDs, DateTime? start = null, DateTime? end = null)
        {
            //var validTime = new DateTime(1900, 1, 1);
            //return Collection.CountDocuments(t => ClassSubjectIDs.Contains(t.ClassSubjectID) && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= time));
            var validTime = new DateTime(1900, 1, 1);
            if (start == null && end == null)
                return Collection.CountDocuments(t => ClassSubjectIDs.Contains(t.ClassSubjectID) && t.Type == SCHEDULE_TYPE.EXAM);
            else if (start == null)
                return Collection.CountDocuments(t => ClassSubjectIDs.Contains(t.ClassSubjectID) && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= end));
            else if (end == null)
                return Collection.CountDocuments(t => ClassSubjectIDs.Contains(t.ClassSubjectID) && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || t.StartDate >= start));
            else
                return Collection.CountDocuments(t => ClassSubjectIDs.Contains(t.ClassSubjectID) && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || (t.StartDate >= start && t.StartDate <= end)));
        }

        public List<LessonScheduleEntity> GetClassSubjectExam(string ClassSubjectID, DateTime? start = null, DateTime? end = null)
        {
            //var validTime = new DateTime(1900, 1, 1);
            //return Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= time)).SortByDescending(t => t.StartDate).ToList();
            var validTime = new DateTime(1900, 1, 1);
            IFindFluent<LessonScheduleEntity, LessonScheduleEntity> data;
            if (start == null && end == null)
                data = Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.Type == SCHEDULE_TYPE.EXAM);
            else if (start == null)
                data = Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= end));
            else if (end == null)
                data = Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || t.StartDate >= start));
            else
                data = Collection.Find(t => t.ClassSubjectID == ClassSubjectID && t.Type == SCHEDULE_TYPE.EXAM && (t.StartDate <= validTime || (t.StartDate >= start && t.StartDate <= end)));
            return data.SortByDescending(t => t.StartDate).ToList();
        }

        public IEnumerable<LessonScheduleEntity> GetByClassSubject(string ClassSubjectID)
        {
            return Collection.Find(t => t.ClassSubjectID == ClassSubjectID).ToEnumerable();
        }

        public async Task RemoveClassSubject(string ClassSubjectID)
        {
            await Collection.DeleteManyAsync(o => o.ClassSubjectID == ClassSubjectID);
        }

        public async Task RemoveManyClass(string[] ids)
        {
            await Collection.DeleteManyAsync(o => ids.Contains(o.ClassID));
        }

        public IEnumerable<LessonScheduleEntity> GetActiveLesson(DateTime StartTime,DateTime EndTime,String ClassSubjectID,String ClassID = "")
        {
            //if (String.IsNullOrEmpty(ClassID))
            //{
                return Collection.Find(o => o.StartDate <= EndTime && o.EndDate >= StartTime && o.ClassSubjectID == ClassSubjectID).ToEnumerable();
            //}
            //else
            //{
            //    return Collection.Find(o => o.StartDate <= EndTime && o.EndDate >= StartTime && o.ClassSubjectID == ClassSubjectID && o.ClassID == ClassID).ToEnumerable();
            //}
        }
    }
}
