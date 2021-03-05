﻿using Core_v2.Repositories;
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
        [JsonProperty("Order")]
        public Int32 Order { get; set; }
        [JsonProperty("Level")]
        public String Level { get; set; }
        [JsonProperty("Tags")]
        public String Tags { get; set; }
        //[JsonProperty("Know")] //so cau muc do nhan biet
        //public List<DetailMatrixExam> Know = new List<DetailMatrixExam>();
        //[JsonProperty("Understanding")] //so cau muc do thong hieu
        //public List<DetailMatrixExam> Understanding = new List<DetailMatrixExam>();
        //[JsonProperty("Manipulate")] //so cau muc do van dung
        //public List<DetailMatrixExam> Manipulate = new List<DetailMatrixExam>();
        //[JsonProperty("ManipulateHighly")]
        //public List<DetailMatrixExam> ManipulateHighly = new List<DetailMatrixExam>();
        [JsonProperty("DetailFormat")]
        public List<DetailMatrixExam> DetailFormat = new List<DetailMatrixExam>();
    }

    public class MatrixExamService : ServiceBase<MatrixExamEntity>
    {
        public MatrixExamService(IConfiguration config) : base(config)
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
        public TypeQuiz Know { get; set; }
        [JsonProperty("Understanding")] //so cau muc do thong hieu
        public TypeQuiz Understanding { get; set; }
        [JsonProperty("Manipulate")] //so cau muc do van dung
        public TypeQuiz Manipulate { get; set; }
        [JsonProperty("ManipulateHighly")]
        public TypeQuiz ManipulateHighly { get; set; }
    }

    public class TypeQuiz
    {
        [JsonProperty("Theory")]
        public Int32 Theory { get; set; }
        [JsonProperty("Exercise")]
        public Int32 Exercise { get; set; }
        [JsonProperty("Total")]
        public Int32 Total { get; set; }
    }
}