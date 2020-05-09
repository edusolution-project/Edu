using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class StudentClassViewModel : ClassEntity
    {
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
        [JsonProperty("Thumb")]
        public string Thumb { get; set; }
        [JsonProperty("StudentNumber")]
        public int StudentNumber { get; set; }
        [JsonProperty("CompletePercent")]
        public long CompletePercent { get; set; }
        [JsonProperty("Progress")]
        public ClassProgressEntity Progress { get; set; }
    }
    public class StudentClassViewModelV2 : ClassSubjectEntity
    {
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("Thumb")]
        public string Thumb { get; set; }
        [JsonProperty("StudentNumber")]
        public int StudentNumber { get; set; }
        [JsonProperty("CompletePercent")]
        public int CompletePercent { get; set; }
        [JsonProperty("Progress")]
        public ClassProgressEntity Progress { get; set; }
    }
}
