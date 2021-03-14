﻿using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class MatrixExamViewModel
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
        public Int32 Level { get; set; }
        [JsonProperty("Tags")]
        public List<String> Tags { get; set; }
        [JsonProperty("Know")] //so cau muc do nhan biet
        public TypeQuiz Know { get; set; }
        [JsonProperty("Understanding")] //so cau muc do thong hieu
        public TypeQuiz Understanding { get; set; }
        [JsonProperty("Manipulate")] //so cau muc do van dung
        public TypeQuiz Manipulate { get; set; }
        [JsonProperty("ManipulateHighly")]
        public TypeQuiz ManipulateHighly { get; set; }
        [JsonProperty("UserName")]
        public String UserName { get; set; }
    }
}
