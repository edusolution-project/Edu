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
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
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
    }
    public class LessonProgressService : ServiceBase<LessonProgressEntity>
    {
        private LearningHistoryService _learningHistoryService;

        public LessonProgressService(IConfiguration config, LearningHistoryService learningHistoryService) : base(config)
        {
            _learningHistoryService = learningHistoryService;
        }

        public async Task UpdateLastLearn(LearningHistoryEntity item)
        {
            var currentProgress = GetByClassID_StudentID_LessonID(item.ClassID, item.StudentID, item.LessonID);
            if (currentProgress == null)
            {
                currentProgress = new LessonProgressEntity
                {
                    ClassID = item.ClassID,
                    LessonID = item.LessonID,
                    StudentID = item.StudentID,
                    FirstDate = _learningHistoryService.GetFirstLearnt(item.StudentID, item.LessonID),
                    LastDate = DateTime.Now,
                    TotalLearnt = _learningHistoryService.CountLessonLearnt(item.StudentID, item.LessonID)
                };
                await Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                await Collection.UpdateManyAsync(t => t.ClassID == item.ClassID && t.StudentID == item.StudentID,
                     new UpdateDefinitionBuilder<LessonProgressEntity>()
                     .Inc(t => t.TotalLearnt, 1)
                     .Set(t => t.LastDate, DateTime.Now)
                     );
            }
        }

        public LessonProgressEntity GetByClassID_StudentID_LessonID(string ClassID, string StudentID, string LessonID)
        {
            return CreateQuery().Find(t => t.ClassID == ClassID && t.StudentID == StudentID && t.LessonID == LessonID).FirstOrDefault();
        }
    }
}
