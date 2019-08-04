using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CacheExamEntity : EntityBase
    {
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; } 
        [JsonProperty("ExamID")]
        public string ExamID { get; set; }
        [JsonProperty("Number")]
        public int Number { get; set; }
        [JsonProperty("Type")]
        public int Type { get; set; } //0 ,1 giống templateType
        [JsonProperty("PassTemp")]
        public string PassTemp { get; set; }
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class CacheExamService : ServiceBase<CacheExamEntity>
    {
        public CacheExamService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
