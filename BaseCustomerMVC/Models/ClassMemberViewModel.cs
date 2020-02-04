using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassStudentViewModel : StudentEntity
    {

        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("LastJoinDate")]
        public DateTime LastJoinDate { get; set; }
        [JsonProperty("ClassStatus")]
        public string ClassStatus { get; set; }
        [JsonProperty("LastJoin")]
        public DateTime LastJoin { get; set; } 
        [JsonProperty("Progress")]
        public ClassProgressEntity Progress { get; set; }
        [JsonProperty("Percent")]
        public double Percent { get; set; }
        [JsonProperty("Score")]
        public ScoreStudentEntity Score { get; set; }
    }
}
