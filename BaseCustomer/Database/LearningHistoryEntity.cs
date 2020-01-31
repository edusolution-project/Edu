using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaseCustomerEntity.Database
{
    public class LearningHistoryEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }
        [JsonProperty("ChapterID")]
        public string ChapterID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; } //LessonID
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("LessonPartID")]
        public string LessonPartID { get; set; } //CloneLessonPartID 
        [JsonProperty("QuestionID")]
        public string QuestionID { get; set; } //Questions 
        [JsonProperty("ViewCount")]
        public int ViewCount { get; set; } //State 
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        
    }
    public class LearningHistoryService : ServiceBase<LearningHistoryEntity>
    {
        private ClassProgressService _classProgressService;
        private LessonProgressService _lessonProgressService;
        private ChapterProgressService _chapterProgressService;

        public LearningHistoryService(IConfiguration config, ClassProgressService classProgressService,
            LessonProgressService lessonProgressService,
            ChapterProgressService chapterProgressService) : base(config)
        {
            _classProgressService = classProgressService;
            _chapterProgressService = chapterProgressService;
            _lessonProgressService = lessonProgressService;

            var indexs = new List<CreateIndexModel<LearningHistoryEntity>>
            {
                new CreateIndexModel<LearningHistoryEntity>(
                    new IndexKeysDefinitionBuilder<LearningHistoryEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.LessonID)
                    .Descending(t=> t.ID)
                    ),
                new CreateIndexModel<LearningHistoryEntity>(
                    new IndexKeysDefinitionBuilder<LearningHistoryEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t => t.ChapterID)
                    .Ascending(t => t.LessonID)
                    .Descending(t=> t.ID)
                    ),
                new CreateIndexModel<LearningHistoryEntity>(
                    new IndexKeysDefinitionBuilder<LearningHistoryEntity>()
                    .Ascending(t => t.StudentID)
                    .Ascending(t=> t.LessonPartID)
                    .Ascending(t => t.QuestionID)
                    .Descending(t=> t.ID)
                    ),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public async Task CreateHist(LearningHistoryEntity item)
        {
            List<LearningHistoryEntity> oldItem = null;
            if (!string.IsNullOrEmpty(item.QuestionID))
                oldItem = CreateQuery().Find(o => o.StudentID == item.StudentID
                    && o.LessonPartID == item.LessonPartID
                    && o.QuestionID == item.QuestionID).ToList();
            else
                oldItem = CreateQuery().Find(o => o.StudentID == item.StudentID
                && o.ClassID == item.ClassID
                && o.ClassSubjectID == item.ClassSubjectID
                && o.LessonID == item.LessonID).ToList();

            item.Time = DateTime.Now;
            if (oldItem == null)
            {
                item.ViewCount = 0;
            }
            else
            {
                item.ViewCount = oldItem.Count;
            }
            CreateOrUpdate(item);
            await _classProgressService.UpdateLastLearn(item);
            await _lessonProgressService.UpdateLastLearn(item);
            await _chapterProgressService.UpdateLastLearn(item);
        }

        public List<LearningHistoryEntity> SearchHistory(LearningHistoryEntity item)
        {
            if (item == null) return null;
            if (!string.IsNullOrEmpty(item.QuestionID))
                return CreateQuery().Find(o => o.StudentID == item.StudentID
                    && o.LessonPartID == item.LessonPartID
                    && o.QuestionID == item.QuestionID).ToList();
            else
                return CreateQuery().Find(o => o.StudentID == item.StudentID
                && o.ClassID == item.ClassID
                && o.LessonID == item.LessonID).ToList();
        }

        public long CountHistory(LearningHistoryEntity item)
        {
            if (item == null) return 0;
            if (!string.IsNullOrEmpty(item.QuestionID))
                return CreateQuery().CountDocuments(o => o.StudentID == item.StudentID
                    && o.LessonPartID == item.LessonPartID
                    && o.QuestionID == item.QuestionID);
            else
                return CreateQuery().CountDocuments(o => o.StudentID == item.StudentID
                && o.ClassID == item.ClassID
                && o.LessonID == item.LessonID);
        }

        public long CountLessonLearnt(string StudentID, string LessonID, string ClassID)
        {
            return CountHistory(new LearningHistoryEntity
            {
                StudentID = StudentID,
                ClassID = ClassID,
                LessonID = LessonID
            });
        }

        public DateTime GetLastLearnt(string StudentID, string LessonID)
        {
            return Collection.Find(t => t.StudentID == StudentID && t.LessonID == LessonID).SortByDescending(t => t.ID).Project(t => t.Time).FirstOrDefault();
        }

        public Task RemoveClassHistory(string ClassID)
        {
            _ = Collection.DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _classProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            return Task.CompletedTask;
        }

        public Task RemoveClassSubjectHistory(string ClassSubjectID)
        {
            _ = Collection.DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            _ = _classProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            _ = _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            _ = _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            return Task.CompletedTask;
        }

        public async Task UpdateClassSubject(ClassSubjectEntity classSubject)
        {
            await Collection.UpdateManyAsync(t => t.ClassID == classSubject.ClassID, Builders<LearningHistoryEntity>.Update.Set("ClassSubjectID", classSubject.ID));
        }
    }
}
