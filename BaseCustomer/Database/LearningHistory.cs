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
        [JsonProperty("LessonID")]
        public string LessonID { get; set; } //LessonID
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("LessonPartID")]
        public string LessonPartID { get; set; } //CloneLessonPartID 
        [JsonProperty("QuestionID")]
        public string QuestionID { get; set; } //Questions 
        [JsonProperty("State")]
        public int State { get; set; } //State 
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        
    }
    public class LearningHistoryService : ServiceBase<LearningHistoryEntity>
    {
        private ClassProgressService _classProgressService;

        public LearningHistoryService(IConfiguration config, ClassProgressService classProgressService) : base(config)
        {
            _classProgressService = classProgressService;
        }

        public Task CreateHist(LearningHistoryEntity item)
        {
            var oldItem = CreateQuery().Find(o => o.StudentID == item.StudentID
                && o.LessonID == item.LessonID
                && o.ClassID == item.ClassID
                && o.LessonPartID == item.LessonPartID
                && o.QuestionID == item.QuestionID).ToList();
            if (string.IsNullOrEmpty(item.LessonPartID) && string.IsNullOrEmpty(item.QuestionID))
            {
                oldItem = CreateQuery().Find(o => o.StudentID == item.StudentID
                && o.LessonID == item.LessonID
                && o.ClassID == item.ClassID).ToList();
            }
            item.Time = DateTime.Now;
            if (oldItem == null)
            {
                item.State = 0;
            }
            else
            {
                item.State = oldItem.Count;
            }
            CreateOrUpdate(item);
            _classProgressService.UpdateLastLearn(item);
            return Task.CompletedTask;
        }
    }
}
