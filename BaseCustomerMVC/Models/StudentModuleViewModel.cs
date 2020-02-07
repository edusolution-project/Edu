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

        [JsonProperty("ScheduleStart")]
        public DateTime ScheduleStart { get; set; }

        [JsonProperty("ScheduleEnd")]
        public DateTime ScheduleEnd { get; set; }

        [JsonProperty("LearnStart")]
        public DateTime LearnStart { get; set; }


        [JsonProperty("Skill")]
        public string Skill { get; set; }

        [JsonProperty("LearnLast")]
        public DateTime LearnLast { get; set; }

        [JsonProperty("LearnCount")]
        public long LearnCount { get; set; }

        [JsonProperty("TypeData")]
        public string TypeData { get; set; }
    }
}
