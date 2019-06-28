using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class LessonPartEntity : EntityBase
    {
        [JsonProperty("ParentID")]
        public string ParentID { get; set; } // chính là lessonID
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Timer")]
        public int Timer { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; }
        [JsonProperty("IsExam")]
        public bool IsExam { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
    }

    public class LessonPartService : ServiceBase<LessonPartEntity>
    {
        public LessonPartService(IConfiguration config) : base(config, "LessonParts")
        {

        }
    }
}
