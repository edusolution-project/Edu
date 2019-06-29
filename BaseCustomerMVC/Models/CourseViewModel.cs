using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CourseViewModel:CourseEntity
    {

        public CourseViewModel(CourseEntity item)
        {
            
        }

        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }

    }
}
