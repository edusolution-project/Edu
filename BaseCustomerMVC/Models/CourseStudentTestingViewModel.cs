using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CourseStudentTestingViewModel : CourseEntity
    {
        
        [JsonProperty("ScheduleID")]
        public string ScheduleID { get; set; }

        [JsonProperty("LessonId")]
        public string LessonId { get; set; }

        [JsonProperty("LessonName")]
        public string LessonName { get; set; }

        [JsonProperty("LessonTypeId")]
        public int LessonTypeId { get; set; }

        [JsonProperty("LessonTypeName")]
        public string LessonTypeName { get; set; }

        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }

        [JsonProperty("MulNum")]
        public double MulNum { get; set; }

        [JsonProperty("LessonStartDate")]
        public DateTime LessonStartDate { get; set; }
        [JsonProperty("LessonEndDate")]
        public DateTime LessonEndDate { get; set; }

        [JsonProperty("TestingEndDate")]
        public DateTime TestingEndDate { get; set; }

        [JsonProperty("PointNew")]
        public double PointNew { get; set; }

        [JsonProperty("NumberTesting")]
        public double NumberTesting { get; set; }

        [JsonProperty("TypeData")]
        public string TypeData { get; set; }

    }
}
