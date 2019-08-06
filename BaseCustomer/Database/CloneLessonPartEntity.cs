using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartEntity : LessonPartEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }

    }

    public class CloneLessonPartService : ServiceBase<CloneLessonPartEntity>
    {
        public CloneLessonPartService(IConfiguration config) : base(config)
        {

        }
    }
}
