using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class LessonExtensionEntity : LessonEntity
    {
        [JsonProperty("CodeExam")]
        public String CodeExam { get; set; }
        [JsonProperty("LessonID")]
        public String LessonID { get; set; }
    }

    public class LessonExtensionService : ServiceBase<LessonExtensionEntity>
    {
        public LessonExtensionService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonExtensionEntity>>
            {
                new CreateIndexModel<LessonExtensionEntity>(
                    new IndexKeysDefinitionBuilder<LessonExtensionEntity>()
                    .Ascending(t => t.LessonID)
                    .Ascending(t=> t.CodeExam)
                    ),
            };
        }

        public List<LessonExtensionEntity> GetItemByLessonID(string LessonID)
        {
            return CreateQuery().Find(x => x.LessonID == LessonID).ToList();
        }
    }
}
