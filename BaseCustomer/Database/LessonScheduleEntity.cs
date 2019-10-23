using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        public bool IsActive { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
    }
    public class LessonScheduleService : ServiceBase<LessonScheduleEntity>
    {
        public LessonScheduleService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonScheduleEntity>>
            {
                //ClassID_1_StartDate_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t => t.ClassID)),
                //ClassID_1_LessonID_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t => t.ClassID)
                    .Ascending(t => t.LessonID)),
                //LessonID_1_StartDate_1
                new CreateIndexModel<LessonScheduleEntity>(
                    new IndexKeysDefinitionBuilder<LessonScheduleEntity>()
                    .Ascending(t=> t.LessonID).Ascending(t=> t.StartDate))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
