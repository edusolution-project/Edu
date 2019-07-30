using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonScheduleViewModel : LessonEntity
    {

        [JsonProperty("ScheduleID")]
        public string ScheduleID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("IsView")]
        public bool IsView { get; set; }
        [JsonProperty("StudentJoins")]
        public long StudentJoins { get; set; }

        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
    }
}
