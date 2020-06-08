using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class ChapterProgressEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("ChapterID")]
        public string ChapterID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("Completed")]
        public int Completed { get; set; }
        [JsonProperty("TotalLessons")]
        public long TotalLessons { get; set; }
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("ExamDone")]
        public long ExamDone { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("PracticePoint")]//Avg Point of Non-exam lesson
        public double PracticePoint { get; set; }
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
    }

    public class ChapterProgressService : ServiceBase<ChapterProgressEntity>
    {
        private ChapterService _chapterService;
        private LessonService _lessonService;

        public ChapterProgressService(IConfiguration config
            //, ChapterService chapterService, LessonService lessonService
            ) : base(config)
        {
            //_chapterService = chapterService;
            //_lessonService = lessonService;
            _chapterService = new ChapterService(config);
            _lessonService = new LessonService(config);

            var indexs = new List<CreateIndexModel<ChapterProgressEntity>>
            {
                //ClassID_1
                new CreateIndexModel<ChapterProgressEntity>(
                    new IndexKeysDefinitionBuilder<ChapterProgressEntity>()
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ChapterID)),
                new CreateIndexModel<ChapterProgressEntity>(
                    new IndexKeysDefinitionBuilder<ChapterProgressEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.ChapterID))
            };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task UpdateLastLearn(LessonProgressEntity item)
        {
            var currentObj = _chapterService.GetItemByID(item.ChapterID);
            if (currentObj == null) return;
            var progress = GetItemByChapterID(item.ChapterID, item.StudentID, item.ClassSubjectID);

            if (progress == null)
            {
                //create new progress
                await Collection.InsertOneAsync(new ChapterProgressEntity
                {
                    StudentID = item.StudentID,
                    Completed = 1,
                    TotalLessons = _lessonService.CountChapterLesson(currentObj.ID),
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = currentObj.ID,
                    ParentID = currentObj.ParentID,
                    LastDate = DateTime.Now,
                    LastLessonID = item.LessonID
                });
            }
            else
            {
                var update = new UpdateDefinitionBuilder<ChapterProgressEntity>()
                      .Set(t => t.LastDate, DateTime.Now)
                      .Set(t => t.LastLessonID, item.LessonID);
                if (item.TotalLearnt == 1) //new
                    update = update.Inc(t => t.Completed, 1);

                await Collection.UpdateManyAsync(t => t.ChapterID == currentObj.ID && t.StudentID == item.StudentID && t.ClassSubjectID == item.ClassSubjectID, update);
            }
        }

        public async Task UpdatePoint(LessonProgressEntity item)
        {
            var progress = GetItemByChapterID(item.ChapterID, item.StudentID, item.ClassSubjectID);
            if (progress == null)
            {
                return;
            }
            else
            {
                if (item.Tried == 1 || progress.ExamDone == 0)//new
                    progress.ExamDone++;

                progress.TotalPoint += item.PointChange;
                progress.AvgPoint = progress.TotalPoint / progress.ExamDone;
                await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
            }
        }

        public ChapterProgressEntity GetItemByChapterID(string ChapterID, string StudentID, string ClassSubjectID)
        {
            return CreateQuery().Find(t => t.ChapterID == ChapterID && t.StudentID == StudentID && t.ClassSubjectID == ClassSubjectID).FirstOrDefault();
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ChapterProgressEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
