using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class FormatExamEntity : EntityBase
    {
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Center")]
        public String Center { get; set; }
        [JsonProperty("Order")]
        public Int32 Order { get; set; }
        [JsonProperty("Level")]
        public String Level { get; set; }
        [JsonProperty("Tags")]
        public String Tags { get; set; }
        [JsonProperty("Know")] //so cau muc do nhan biet
        public Int32 Know { get; set; }
        [JsonProperty("Understanding")] //so cau muc do thong hieu
        public Int32 Understanding { get; set; }
        [JsonProperty("Manipulate")] //so cau muc do van dung
        public Int32 Manipulate { get; set; }
        [JsonProperty("ManipulateHighly")]
        public String ManipulateHighly { get; set; }
        [JsonProperty("ExamQuestionArchiveID")] //format de ung voi kho de nao
        public String ExamQuestionArchiveID { get; set; }
    }

    public class FormatExamService : ServiceBase<FormatExamEntity>
    {
        public FormatExamService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<FormatExamEntity>>
            {
                new CreateIndexModel<FormatExamEntity>(
                    new IndexKeysDefinitionBuilder<FormatExamEntity>()
                    .Ascending(t=> t.ID)
                    .Ascending(x=>x.ExamQuestionArchiveID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
