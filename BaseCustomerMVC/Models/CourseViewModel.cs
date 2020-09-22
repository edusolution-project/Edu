using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CourseViewModel : CourseEntity
    {
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("SkillName")]
        public string SkillName { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
    }
}
