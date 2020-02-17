using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class LessonProgressEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
        [JsonProperty("ChapterID")]
        public string ChapterID { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("TotalLearnt")]
        public long TotalLearnt { get; set; }
        [JsonProperty("FirstDate")]
        public DateTime FirstDate { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }
        [JsonProperty("MinPoint")]
        public double MinPoint { get; set; }
        [JsonProperty("LastPoint")]
        public double LastPoint { get; set; }
        [JsonProperty("Tried")]
        public long Tried { get; set; }
        [JsonProperty("LastTry")]
        public DateTime LastTry { get; set; }
        [JsonProperty("PointChange")]
        public double PointChange { get; set; }
    }
    public class LessonProgressService : ServiceBase<LessonProgressEntity>
    {
        private LessonService _lessonService;
        public LessonProgressService(IConfiguration config) : base(config)
        {
            _lessonService = new LessonService(config);
        }

        public async Task UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentProgress = GetByClassSubjectID_StudentID_LessonID(item.ClassSubjectID, item.StudentID, item.LessonID);
            if (currentProgress == null)
            {
                currentProgress = new LessonProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = item.ChapterID,
                    LessonID = item.LessonID,
                    StudentID = item.StudentID,
                    LastDate = DateTime.Now,
                    FirstDate = DateTime.Now,
                    TotalLearnt = 1,
                };
                await Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                await Collection.UpdateManyAsync(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID && t.LessonID == item.LessonID,
                     new UpdateDefinitionBuilder<LessonProgressEntity>()
                     .Inc(t => t.TotalLearnt, 1)
                     .Set(t => t.LastDate, DateTime.Now)
                     );
            }
        }

        public async Task UpdateLastPoint(ExamEntity item)
        {
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null) return;
            var currentProgress = GetByClassSubjectID_StudentID_LessonID(item.ClassSubjectID, item.StudentID, item.LessonID);
            if (currentProgress == null)
            {
                currentProgress = new LessonProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = lesson.ChapterID,
                    LessonID = item.LessonID,
                    StudentID = item.StudentID,
                    LastDate = DateTime.Now,
                    FirstDate = DateTime.Now,
                    TotalLearnt = 1,
                    AvgPoint = item.Point,
                    LastPoint = item.Point,
                    MaxPoint = item.Point,
                    MinPoint = item.Point,
                    Tried = item.Number,
                    LastTry = item.Updated,
                    PointChange = item.Point
                };
                await Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var avg = currentProgress.AvgPoint * currentProgress.Tried;
                if (currentProgress.Tried > item.Number)
                    currentProgress.Tried++;
                else
                    currentProgress.Tried = item.Number;

                //convert point => percent
                var point = item.MaxPoint == 0 ? 0 : (item.Point * 100 / item.MaxPoint);
                currentProgress.PointChange = point - currentProgress.LastPoint;
                currentProgress.LastPoint = point;
                currentProgress.LastTry = item.Updated;
                if (point > currentProgress.MaxPoint) currentProgress.MaxPoint = point;
                if (point < currentProgress.MinPoint) currentProgress.MinPoint = point;
                avg = (avg + point) / currentProgress.Tried;
                currentProgress.AvgPoint = avg;
                await Collection.ReplaceOneAsync(t => t.ID == currentProgress.ID, currentProgress);
            }
        }


        public LessonProgressEntity GetByClassSubjectID_StudentID_LessonID(string ClassSubjectID, string StudentID, string LessonID)
        {
            return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID && t.LessonID == LessonID).FirstOrDefault();
        }


        public List<LessonProgressEntity> GetByClassID_StudentID(string ClassID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID).SortByDescending(t => t.LastDate).ToList();
        }

        public List<LessonProgressEntity> GetByClassSubjectID_StudentID(string ClassSubjectID, string StudentID)
        {
            return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID).SortByDescending(t => t.LastDate).ToList();
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<LessonProgressEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }


    }
}
