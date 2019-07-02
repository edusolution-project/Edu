﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartAnswerEntity : LessonPartAnswerEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
    }
    public class CloneLessonPartAnswerService : ServiceBase<CloneLessonPartAnswerEntity>
    {
        public CloneLessonPartAnswerService(IConfiguration config) : base(config)
        {

        }
    }
}
