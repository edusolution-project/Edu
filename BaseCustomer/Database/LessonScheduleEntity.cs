using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class LessonScheduleEntity : EntityBase
    {
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsActive")]
        public DateTime IsActive { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
    }
    public class LessonScheduleService : ServiceBase<LessonScheduleEntity>
    {
        public LessonScheduleService(IConfiguration config) : base(config)
        {

        }
    }
}
