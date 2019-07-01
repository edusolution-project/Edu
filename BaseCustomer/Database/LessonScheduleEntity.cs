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
        public DateTime LessonID { get; set; }
        [JsonProperty("OpeningDate")]
        public DateTime OpeningDate { get; set; }
        [JsonProperty("CloseDate")]
        public DateTime CloseDate { get; set; }
        [JsonProperty("ClassID")]
        public DateTime ClassID { get; set; }
    }
    public class LessonScheduleService : ServiceBase<LessonEntity>
    {
        public LessonScheduleService(IConfiguration config) : base(config)
        {

        }
    }
}
