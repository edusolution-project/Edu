using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class StudentProgressEntity : EntityBase
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }

        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }

        [JsonProperty("CompletedLessons")]
        public int CompletedLessons { get; set; }
        [JsonProperty("TotalLessons")]
        public int TotalLessons { get; set; }
        [JsonProperty("LastView")]
        public DateTime LastView { get; set; }
        [JsonProperty("StudentID")]
        public int StudentID { get; set; }
    }
    public class StudentProgressService : ServiceBase<StudentProgressEntity>
    {
        public StudentProgressService(IConfiguration config) : base(config)
        {

        }
    }
}
