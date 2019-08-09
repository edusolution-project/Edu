using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerMVC.Models
{
    public class TeacherExamViewModel : ExamEntity
    {
        [JsonProperty("Parts")]
        public List<ExamPartViewModel> Parts { get; set; } = new List<ExamPartViewModel>() { };
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("LessonName")]
        public string LessonName { get; set; }
        [JsonProperty("Multiple")]
        public double Multiple { get; set; }

        [JsonProperty("MarkDate")]
        public DateTime MarkDate { get; set; }

        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
    }
}
