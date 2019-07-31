using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LearningHistoryViewModel : LearningHistoryEntity
    {

        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("LessonName")]
        public string LessonName { get; set; }
    }
}
