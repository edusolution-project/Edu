using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class ScoreEntity : EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }

        [JsonProperty("ClassID")]
        public string ClassID { get; set; }

        [JsonProperty("LessonID")]
        public string LessonID { get; set; }

        [JsonProperty("TeacherID")]
        public DateTime TeacherID { get; set; }
        
        [JsonProperty("ScoreType")]
        public int ScoreType { get; set; }

        [JsonProperty("Multiple")]
        public int Multiple { get; set; }

        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
    }
    
    public class ScoreService : ServiceBase<ScoreEntity>
    {
        public ScoreService(IConfiguration config) : base(config)
        {

        }

    }
}
