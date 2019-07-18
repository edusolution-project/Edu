using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        [JsonProperty("Questions")]
        public int Questions { get; set; } //Questions 
        [JsonProperty("State")]
        public int State { get; set; } //State 
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        
    }
    public class LearningHistoryService : ServiceBase<LearningHistoryEntity>
    {
        public LearningHistoryService(IConfiguration config) : base(config)
        {

        }
    }
}
