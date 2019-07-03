using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class MyClassViewModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("CourseID")]
        public string SubjectName { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("Status")]
        public bool Status { get; set; }
        [JsonProperty("StudentNumber")]
        public int StudentNumber { get; set; }
    }
}
