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
        [JsonProperty("OriginID")]
        public string OriginID { get; set; }

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
        //[JsonProperty("IsExam")]
        //public bool IsExam { get; set; }
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
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
    }

    public class LessonPartService : ServiceBase<LessonPartEntity>
    {
        private IConfiguration config;
        private string dbName;

        public LessonPartService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonPartEntity>>
            {
                //CourseID_1
                new CreateIndexModel<LessonPartEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartEntity>()
                    .Ascending(t => t.CourseID)),
                //ParentID_1
                new CreateIndexModel<LessonPartEntity>(
                    new IndexKeysDefinitionBuilder<LessonPartEntity>()
                    .Ascending(t=> t.ParentID))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public LessonPartService(IConfiguration config, string dbName) : base(config, dbName)
        {
        }

        public IEnumerable<LessonPartEntity> GetByLessonID(string LessonID)
        {
            return CreateQuery().Find(o => o.ParentID == LessonID)
                                .SortBy(q => q.Order)
                                .ThenBy(q => q.ID).ToEnumerable();
        }

        //TODO: CHECK PERFORMANCE
        public IEnumerable<LessonPartEntity> GetItemByTypeQuiz_LessonIDs(List<string> LessonIDs)
        {
            return CreateQuery().Find(x => LessonIDs.Contains(x.ParentID) && x.Type.ToUpper().Contains("QUIZ")).ToEnumerable();
        }
    }
}
