using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ExamQuestionArchiveViewModel : ExamQuestionArchiveEntity
    {
        [JsonProperty("SubjectName")]
        public String SubjectName { get; set; }
        [JsonProperty("GradeName")]
        public String GradeName { get; set; }
        [JsonProperty("TotalQuestion")]
        public Int32 TotalQuestion { get; set; }
        [JsonProperty("MainSubjectName")]
        public String MainSubjectName { get; set; }
    }
}
