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
        public int CompletedLessons { get; set; } //LessonID
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; } //CloneLessonPartID
        [JsonProperty("LastView")]
        public DateTime LastView { get; set; }
        [JsonProperty("StudentID")]
        public int StudentID { get; set; }
    }
    public class ClassProgressService : ServiceBase<ClassProgressEntity>
    {
        public ClassProgressService(IConfiguration config) : base(config)
        {

        }
    }
}
