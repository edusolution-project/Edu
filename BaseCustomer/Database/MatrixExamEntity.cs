using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class MatrixExamEntity : EntityBase
    {
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("ExamQuestionArchiveID")] //format de ung voi kho de nao
        public String ExamQuestionArchiveID { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Center")]
        public String Center { get; set; }
        //[JsonProperty("Order")]
        //public Int32 Order { get; set; }
        //[JsonProperty("Level")]
        //public String Level { get; set; }
        //[JsonProperty("Tags")]
        //public String Tags { get; set; }
        //[JsonProperty("Know")] //so cau muc do nhan biet
        //public List<DetailFormat> Know = new List<DetailFormat>();
        //[JsonProperty("Understanding")] //so cau muc do thong hieu
        //public List<DetailFormat> Understanding = new List<DetailFormat>();
        //[JsonProperty("Manipulate")] //so cau muc do van dung
        //public List<DetailFormat> Manipulate = new List<DetailFormat>();
        //[JsonProperty("ManipulateHighly")]
        //public List<DetailFormat> ManipulateHighly = new List<DetailFormat>();
        [JsonProperty("DetailFormat")]
        public List<DetailMatrixExam> DetailFormat = new List<DetailMatrixExam>();
    }

    public class FormatExamService : ServiceBase<MatrixExamEntity>
    {
        public FormatExamService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<MatrixExamEntity>>
            {
                new CreateIndexModel<MatrixExamEntity>(
                    new IndexKeysDefinitionBuilder<MatrixExamEntity>()
                    .Ascending(t=> t.ID)
                    .Ascending(x=>x.ExamQuestionArchiveID)
                    )
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }
    }

    public class DetailMatrixExam
    {
        [JsonProperty("Order")]
        public Int32 Order { get; set; }
        [JsonProperty("Level")]
        public Int32 Level { get; set; }
        [JsonProperty("Tags")]
        public String Tags { get; set; }
        [JsonProperty("Total")]
        public Int32 Total { get; set; }
        [JsonProperty("Know")] //so cau muc do nhan biet
        public Int32 Know { get; set; }
        [JsonProperty("Understanding")] //so cau muc do thong hieu
        public Int32 Understanding { get; set; }
        [JsonProperty("Manipulate")] //so cau muc do van dung
        public Int32 Manipulate { get; set; }
        [JsonProperty("ManipulateHighly")]
        public Int32 ManipulateHighly { get; set; }
    }
}
