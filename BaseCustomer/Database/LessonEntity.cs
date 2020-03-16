using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class LessonEntity : CourseLessonEntity
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }

        [JsonProperty("ClassSubjectID")]
        public string ClassSubjectID { get; set; }

    }

    public class LESSON_TEMPLATE
    {
        public const int LECTURE = 1, EXAM = 2;
    }

    public class LESSON_ETYPE
    {
        public const int PRACTICE = 0, WEEKLY = 1, CHECKPOINT = 2, EXPERIMENT = 3, INTERSHIP = 4, END = 5;
    }

    public class LessonService : ServiceBase<LessonEntity>
    {
        public LessonService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonEntity>>
            {
                //CourseID_1_ChapterID_1_Order_1_ID_1
                new CreateIndexModel<LessonEntity>(
                    new IndexKeysDefinitionBuilder<LessonEntity>()
                    .Ascending(t => t.CourseID)
                    .Ascending(t=> t.ChapterID)),
                //ChapterID_1_Order_1
                new CreateIndexModel<LessonEntity>(
                    new IndexKeysDefinitionBuilder<LessonEntity>()
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
    }
}
