using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class LessonExamEntity : LessonEntity
    {
        [JsonProperty("CodeExam")]
        public String CodeExam { get; set; }
        //[JsonProperty("LessonID")]
        //public String LessonID { get; set; }
        [JsonProperty("MatrixExamID")]
        public String MatrixExamID { get; set; }
        [JsonProperty("ManageExamID")]
        public String ManageExamID { get; set; }
    }

    public class LessonExamService : ServiceBase<LessonExamEntity>
    {
        public LessonExamService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<LessonExamEntity>>
            {
                new CreateIndexModel<LessonExamEntity>(
                    new IndexKeysDefinitionBuilder<LessonExamEntity>()
                    //.Ascending(t => t.LessonID)
                    .Ascending(t=> t.CodeExam)
                    ),
            };
        }

        //public List<LessonExamEntity> GetItemByLessonID(string LessonID)
        //{
        //    return CreateQuery().Find(x => x.LessonID == LessonID).ToList();
        //}

        public List<LessonExamEntity> GetItemsByManageExamID(String ID)
        {
            return CreateQuery().Find(x=>x.ManageExamID == ID).ToList();
        }
    }
}
