using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class CloneLessonPartQuestionEntity : LessonPartQuestionEntity
    {
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
    }
    public class CloneLessonPartQuestionService : ServiceBase<CloneLessonPartQuestionEntity>
    {
        public CloneLessonPartQuestionService(IConfiguration config) : base(config)
        {

        }
    }

}
