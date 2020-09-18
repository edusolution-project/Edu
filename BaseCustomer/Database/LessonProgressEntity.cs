using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

            var indexs = new List<CreateIndexModel<LessonProgressEntity>>
            {
                //ClassID_1_StudentID_1
                new CreateIndexModel<LessonProgressEntity>(
                    new IndexKeysDefinitionBuilder<LessonProgressEntity>()
                    .Ascending(t => t.ClassID)
                    .Ascending(t=> t.StudentID)
                    ),
                //ClassSubjectID_1_StudentID_1
                new CreateIndexModel<LessonProgressEntity>(
                    new IndexKeysDefinitionBuilder<LessonProgressEntity>()
                    .Ascending(t => t.ClassSubjectID)
                    .Ascending(t=> t.StudentID)
                    ),
                //StudentID_1_LessonID_1
                new CreateIndexModel<LessonProgressEntity>(
                    new IndexKeysDefinitionBuilder<LessonProgressEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t=> t.LessonID)
                    )

            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentProgress = GetByStudentID_LessonID( item.StudentID, item.LessonID);
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
                await Collection.UpdateManyAsync(t => t.StudentID == item.StudentID && t.LessonID == item.LessonID,
                     new UpdateDefinitionBuilder<LessonProgressEntity>()
                     .Inc(t => t.TotalLearnt, 1)
                     .Set(t => t.LastDate, DateTime.Now)
                     );
            }
        }

        public async Task<LessonProgressEntity> UpdateLastPoint(ExamEntity item)
        {
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null) return null;
            var currentProgress = GetByStudentID_LessonID(item.StudentID, item.LessonID);
            //New: use real point, not count question
            var point = item.MaxPoint > 0 ? (item.Point * 100.0 / item.MaxPoint) : 0;
            if (currentProgress == null)
            {
                //var point = item.QuestionsTotal > 0 ? (item.QuestionsPass * 100.0 / item.QuestionsTotal) : 0;
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
                    AvgPoint = point,
                    LastPoint = point,
                    MaxPoint = point,
                    MinPoint = point,
                    Tried = item.Number,
                    LastTry = item.Updated,
                    PointChange = point
                };
                await Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var avg = currentProgress.AvgPoint * currentProgress.Tried;

                if (item.Number > currentProgress.Tried)//new 
                {
                    currentProgress.Tried = item.Number;
                    currentProgress.LastTry = item.Updated;

                    currentProgress.PointChange = point - currentProgress.LastPoint;
                    currentProgress.LastPoint = point;
                    currentProgress.AvgPoint = (avg + point) / currentProgress.Tried;
                }
                else
                {
                    var pointchange = item.MaxPoint > 0 ? (item.Point - item.LastPoint) * 100 / item.MaxPoint : 0;
                    if (item.Number == currentProgress.Tried)//lastest 
                    {
                        currentProgress.PointChange += pointchange;
                        currentProgress.LastPoint = point;
                    }
                    currentProgress.AvgPoint = (avg + pointchange) / currentProgress.Tried; //lastest && old exam
                }

                if (point > currentProgress.MaxPoint) currentProgress.MaxPoint = point;
                if (point < currentProgress.MinPoint) currentProgress.MinPoint = point;

                await Collection.ReplaceOneAsync(t => t.ID == currentProgress.ID, currentProgress);
            }
            return currentProgress;
        }


        //public LessonProgressEntity GetByClassSubjectID_StudentID_LessonID(string ClassSubjectID, string StudentID, string LessonID)
        //{
        //    return CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID && t.StudentID == StudentID && t.LessonID == LessonID).FirstOrDefault();
        //}

        public LessonProgressEntity GetByStudentID_LessonID(string StudentID, string LessonID)
        {
            return CreateQuery().Find(t => t.StudentID == StudentID && t.LessonID == LessonID).FirstOrDefault();
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

        public void ResetPoint(LessonProgressEntity item)
        {
            Collection.UpdateMany(t => t.ID == item.ID, Builders<LessonProgressEntity>.Update.Set(t => t.LastPoint, 0));
        }
    }
}
