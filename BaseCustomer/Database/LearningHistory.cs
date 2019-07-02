using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class LearningHistoryEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; } //LessonID
        [JsonProperty("LessonPartID")]
        public bool LessonPartID { get; set; } //CloneLessonPartID
        [JsonProperty("QuestionID")]
        public int QuestionID { get; set; }
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        [JsonProperty("StudentID")]
        public int StudentID { get; set; }
    }
    public class LearningHistoryService : ServiceBase<LearningHistoryEntity>
    {
        public LearningHistoryService(IConfiguration config) : base(config)
        {

        }
    }
}
