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
        [JsonProperty("CompletedLessons")]
        public List<string> CompletedLessons { get; set; }
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; }
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
    }

    public class ChapterProgressService : ServiceBase<ChapterProgressEntity>
    {
        private ChapterService _chapterService;
        private LessonService _lessonService;

        public ChapterProgressService(IConfiguration config, ChapterService chapterService, LessonService lessonService) : base(config)
        {
            _chapterService = chapterService;
            _lessonService = lessonService;

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

        public ChapterProgressEntity GetItemByChapterID(string ChapterID, string StudentID, string ClassID)
        {
            return CreateQuery().Find(t => t.ChapterID == ChapterID && t.StudentID == StudentID && t.ClassID == ClassID).FirstOrDefault();
        }

        public async Task UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentChapter = _chapterService.GetItemByID(item.ChapterID);
            var progress = GetItemByChapterID(item.ChapterID, item.StudentID, item.ClassID);

            if (currentChapter != null)
            {
                if (progress == null)
                {
                    //create new progress
                    await Collection.InsertOneAsync(new ChapterProgressEntity
                    {
                        StudentID = item.StudentID,
                        CompletedLessons = new List<string>() { item.LessonID },
                        TotalLessons = (int)_lessonService.CreateQuery().CountDocuments(t => t.ChapterID == currentChapter.ID),
                        ClassID = item.ClassID,
                        ChapterID = currentChapter.ID,
                        ParentID = currentChapter.ParentID,
                        LastDate = DateTime.Now,
                        LastLessonID = item.LessonID
                    });
                }
                else
                {
                    var update = Builders<ChapterProgressEntity>.Update;

                    var updates = new List<UpdateDefinition<ChapterProgressEntity>>();

                    if (!progress.CompletedLessons.Contains(item.LessonID))
                    {
                        updates.Add(update.AddToSet(t => t.CompletedLessons, item.LessonID));
                        //update parent chapter
                        //if (!string.IsNullOrEmpty(currentChapter.ParentID) && currentChapter.ParentID != "0")
                        //{
                        //    var parentChap = _chapterService.GetItemByID(currentChapter.ParentID);
                        //    if (parentChap != null)
                        //    {
                        //        if (progress.CompletedLessons.Count == progress.TotalLessons - 1)
                        //        {
                        //        }
                        //    }
                        //}
                    }
                    updates.Add(update.Set(t => t.LastDate, DateTime.Now)
                           .Set(t => t.LastLessonID, item.LessonID));

                    var r = await Collection.UpdateManyAsync(t => t.ChapterID == item.ChapterID && t.StudentID == item.StudentID,
                                update.Combine(updates));
                }
            }
        }

    }
}
