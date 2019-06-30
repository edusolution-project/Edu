using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CourseViewModel:CourseEntity
    {
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }

    }
}
