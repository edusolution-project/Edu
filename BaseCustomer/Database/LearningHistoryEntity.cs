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
        private ChapterProgressService _chapterProgressService;

        public LearningHistoryService(IConfiguration config, ClassProgressService classProgressService, ChapterProgressService chapterProgressService) : base(config)
        {
            _classProgressService = classProgressService;
            _chapterProgressService = chapterProgressService;

            var indexs = new List<CreateIndexModel<LearningHistoryEntity>>
            {
                //StudentID_1_ClassID_1_LessonID_1_ID_-1
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
            await _chapterProgressService.UpdateLastLearn(item);
        }
    }
}
