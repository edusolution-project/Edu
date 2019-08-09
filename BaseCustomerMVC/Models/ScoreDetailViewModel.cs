using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class ScoreDetailViewModel : ScoreEntity
    {
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }

        [JsonProperty("ExamID")]
        public string ExamID { get; set; }

        [JsonProperty("LessonName")]
        public string LessonName { get; set; }

    }
}
