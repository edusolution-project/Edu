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
        private IConfiguration config;
        private string dbName;

        //private LessonProgressService _lessonProgressService;
        //private ChapterProgressService _chapterProgressService;
        //private ClassSubjectProgressService _classSubjectProgressService;
        //private ClassProgressService _classProgressService;

        public LearningHistoryService(IConfiguration config) : base(config)
        {

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

        public LearningHistoryService(IConfiguration config, string dbName) : base(config, dbName)
        {
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
                //&& o.ClassID == item.ClassID
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

        public LearningHistoryEntity GetLastLearnt(string StudentID, string LessonID, string ClassSubjectID)
        {
            return Collection.Find(t => t.StudentID == StudentID && t.LessonID == LessonID && t.ClassSubjectID == ClassSubjectID).FirstOrDefault();
        }

        public LearningHistoryEntity GetStudentLastLearn(string StudentID)
        {
            return Collection.Find(t => t.StudentID == StudentID).SortByDescending(t => t.ID).FirstOrDefault();
        }
    }
}
