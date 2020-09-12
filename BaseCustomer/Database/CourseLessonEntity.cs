using Core_v2.Globals;
using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class CourseLessonEntity : EntityBase
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
        [JsonProperty("IsPractice")]  //TEMPLATE TYPE == LECTURE && HAS QUIZ
        public bool IsPractice { get; set; }
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

    public class CourseLessonService : ServiceBase<CourseLessonEntity>
    {
        public CourseLessonService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<CourseLessonEntity>>
            {
                //CourseID_1_ChapterID_1_Order_1_ID_1
                new CreateIndexModel<CourseLessonEntity>(
                    new IndexKeysDefinitionBuilder<CourseLessonEntity>()
                    .Ascending(t => t.CourseID)
                    .Ascending(t=> t.ChapterID)),
                //ChapterID_1_Order_1
                new CreateIndexModel<CourseLessonEntity>(
                    new IndexKeysDefinitionBuilder<CourseLessonEntity>()
                    .Ascending(t=> t.ChapterID).Ascending(t=> t.Order))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public long CountChapterLesson(string ChapterID)
        {
            return Collection.CountDocumentsAsync(t => t.ChapterID == ChapterID).Result;
        }

        public long CountCourseLesson(string CourseID)
        {
            return Collection.CountDocumentsAsync(t => t.CourseID == CourseID).Result;
        }

        public IEnumerable<CourseLessonEntity> GetChapterLesson(string ChapterID)
        {
            return Collection.Find(t => t.ChapterID == ChapterID).ToEnumerable();
        }

        public IEnumerable<CourseLessonEntity> GetChapterLesson(string CourseID, string ChapterID)
        {
            if (!string.IsNullOrEmpty(ChapterID) && ChapterID != "0")
                return GetChapterLesson(ChapterID);
            return Collection.Find(t => t.CourseID == CourseID && t.ChapterID == "0").SortBy(t => t.Order).ToEnumerable();
        }

        public void UpdateLessonPoint(string ID, double point)
        {
            CreateQuery().UpdateOne(t => t.ID == ID, Builders<CourseLessonEntity>.Update.Set(t => t.Point, point));
        }
    }
}
