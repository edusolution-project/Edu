using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class ChapterProgressViewModel : ChapterEntity
    {
        [JsonProperty("CompletedLessons")]
        public int CompletedLessons { get; set; }
        [JsonProperty("LastLessonID")]
        public string LastLessonID { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
        [JsonProperty("PracticeAvgPoint")]
        public double PracticeAvgPoint { get; set; }
    }
}
