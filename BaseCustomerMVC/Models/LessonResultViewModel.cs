using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonResultViewModel : LessonEntity
    {
        [JsonProperty("ScheduleID")]
        public string ScheduleID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("LearntCount")]
        public long LearntCount { get; set; }
        [JsonProperty("AvgPracticePoint")]
        public double AvgPracticePoint { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
    }
}
