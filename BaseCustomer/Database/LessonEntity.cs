using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;

namespace BaseCustomerEntity.Database
{
    public class LessonEntity : EntityBase
    {
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("ChapterID")]
        public string ChapterID { get; set; }
        [JsonProperty("IsParentCourse")]
        public bool IsParentCourse { get; set; } // có phải là course hay không ?
        [JsonProperty("TemplateType")]
        public int TemplateType { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("Timer")]
        public int Timer { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("IsAdmin")]
        public bool IsAdmin { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("Limit")] // gioi han so luot lam bai
        public int Limit { get; set; }
        [JsonProperty("Multiple")] // hệ số 
        public double Multiple { get; set; }

        [JsonProperty("Etype")] // kiểu bài thi (thành phần / cuối kì) 
        public int Etype { get; set; }
    }

    public class LESSON_TEMPLATE
    {
        public const int LECTURE = 1, EXAM = 2;
    }

    public class LESSON_ETYPE
    {
        public const int PARTIAL = 1, END = 2;
    }

    public class LessonService : ServiceBase<LessonEntity>
    {
        public LessonService(IConfiguration config) : base(config)
        {

        }
    }
}
