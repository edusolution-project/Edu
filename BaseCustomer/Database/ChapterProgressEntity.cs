using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
        [JsonProperty("PracticeCount")]//Count of Completed Non-exam lesson 
        public double PracticeCount { get; set; }
        [JsonProperty("PracticeAvgPoint")]//Avg Point of Non-exam lesson
        public double PracticeAvgPoint { get; set; }
        [JsonProperty("PracticePoint")]//Total Point of Non-exam lesson
        public double PracticePoint { get; set; }


    }

    public class ChapterProgressService : ServiceBase<ChapterProgressEntity>
    {
        private ChapterService _chapterService;
        private LessonService _lessonService;
        private CloneLessonPartService _lessonPartService;

        public ChapterProgressService(IConfiguration config
            //, ChapterService chapterService, LessonService lessonService
            ) : base(config)
        {
            //_chapterService = chapterService;
            //_lessonService = lessonService;
            _chapterService = new ChapterService(config);
            _lessonService = new LessonService(config);
            _lessonPartService = new CloneLessonPartService(config);

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
                progress = NewProgressEntity(currentObj, item.StudentID);
                progress.LastLessonID = item.ID;
                //create new progress
                await Collection.InsertOneAsync(progress);
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

        public async Task UpdatePracticePoint(LessonProgressEntity item)
        {
            var progress = GetItemByChapterID(item.ChapterID, item.StudentID, item.ClassSubjectID);
            if (progress == null)
            {
                progress = NewProgressEntity(_chapterService.GetItemByID(item.ChapterID), item.StudentID);
                progress.LastLessonID = item.ID;
                progress.PracticePoint += item.PointChange;
                //progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeCount;
                await Collection.InsertOneAsync(progress);
            }
            else
            {
                var init = !(progress.PracticePoint > 0);
                progress.PracticePoint += item.PointChange;
                //progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeCount;
                await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
            }
            await UpdatePracticePoint(progress, item.PointChange);
        }

        public async Task UpdatePracticePoint(ChapterProgressEntity item, double pointchange)
        {
            var progress = GetItemByChapterID(item.ParentID, item.StudentID, item.ClassSubjectID);
            if (progress == null)
            {
                var parentChap = _chapterService.GetItemByID(item.ParentID);
                if (parentChap == null) return;
                progress = NewProgressEntity(parentChap, item.StudentID);
                progress.PracticePoint += pointchange;
                //progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeCount;
                await Collection.InsertOneAsync(progress);
                await UpdatePracticePoint(progress, pointchange);
            }
            else
            {
                //Practice Point = All Subchap Practice Point + All direct lesson practice Point
                //var supchaps_count = _chapterService.GetSubChapters(item.ClassSubjectID, item.ID).Count;
                //var supchaps_point = Collection.Aggregate()
                //    .Match(t=> t.ParentID == item.ID)
                //    .Group(
                //        t => t.ParentID,
                //        group => new
                //        {
                //            ParentID = group.Key,
                //            Sum = group.Sum(t => t.PracticePoint)
                //        }
                //    ).Project(t=> t.Sum);
                progress.PracticePoint += pointchange;
                //progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeCount;

                await Collection.ReplaceOneAsync(t => t.ID == progress.ID, progress);
                await UpdatePracticePoint(progress, pointchange);
            }
        }

        public ChapterProgressEntity GetItemByChapterID(string ChapterID, string StudentID, string ClassSubjectID)
        {
            return CreateQuery().Find(t => t.ChapterID == ChapterID && t.StudentID == StudentID
            //&& t.ClassSubjectID == ClassSubjectID => don't need now
            ).FirstOrDefault();
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<ChapterProgressEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }

        public ChapterProgressEntity NewProgressEntity(ChapterEntity chapter, string StudentID)
        {
            return new ChapterProgressEntity
            {
                StudentID = StudentID,
                Completed = 1,
                TotalLessons = _lessonService.CountChapterLesson(chapter.ID),
                //PracticeCount = CountChapterPractice(chapter.ID, chapter.ClassSubjectID),
                PracticeCount = chapter.PracticeCount,
                ClassID = chapter.ClassID,
                ClassSubjectID = chapter.ClassSubjectID,
                ChapterID = chapter.ID,
                ParentID = chapter.ParentID,
                LastDate = DateTime.Now,
                //LastLessonID = lessonPrg.LessonID
            };
        }

        //private int CountChapterPractice(string chapterID, string classSubjectID)
        //{
        //    var result = 0;
        //    var lessonids = _lessonService.CreateQuery().Find(t => t.TemplateType == LESSON_TEMPLATE.LECTURE && t.ChapterID == chapterID).Project(t => t.ID).ToEnumerable();
        //    if (lessonids != null && lessonids.Count() > 0)
        //    {
        //        var quizList = new List<string> { "QUIZ1", "QUIZ2", "QUZI3", "ESSAY" };
        //        foreach (var id in lessonids)
        //        {
        //            if (_lessonPartService.CreateQuery().Find(t => t.ParentID == id && quizList.Contains(t.Type)).CountDocuments() > 0)
        //                result++;
        //        }
        //    }
        //    var subchaps = _chapterService.GetSubChapters(classSubjectID, chapterID);
        //    if (subchaps != null && subchaps.Count > 0)
        //    {
        //        foreach (var chap in subchaps)
        //            result += CountChapterPractice(chap.ID, chap.ClassSubjectID);
        //    }
        //    return result;

        //}
    }
}
