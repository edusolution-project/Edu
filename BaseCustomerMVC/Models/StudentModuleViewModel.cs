using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class StudentModuleViewModel : LessonEntity
    {
        
        [JsonProperty("ScheduleID")]
        public string ScheduleID { get; set; }

        [JsonProperty("LessonId")]
        public string LessonId { get; set; }

        [JsonProperty("LessonName")]
        public string LessonName { get; set; }

        [JsonProperty("LessonStartDate")]
        public DateTime LessonStartDate { get; set; }

        [JsonProperty("LessonEndDate")]
        public DateTime LessonEndDate { get; set; }

        [JsonProperty("LearningStartDate")]
        public DateTime LearningStartDate { get; set; }

        [JsonProperty("LearningEndDate")]
        public DateTime LearningEndDate { get; set; }

        [JsonProperty("LearningNumber")]
        public long LearningNumber { get; set; }

        [JsonProperty("TypeData")]
        public string TypeData { get; set; }
    }
}
