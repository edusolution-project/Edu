using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class ClassProgressEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("CompletedLessons")]
        public int CompletedLessons { get; set; }
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; }
        [JsonProperty("LastLesson")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }

    }
    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        public ClassProgressService(IConfiguration config) : base(config)
        {

        }
    }
}
