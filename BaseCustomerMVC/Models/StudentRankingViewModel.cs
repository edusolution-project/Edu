using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class StudentRankingViewModel
    {
        [JsonProperty("Rank")]
        public int Rank { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
        [JsonProperty("ExamDone")]
        public long ExamDone { get; set; }
        [JsonProperty("Count")]
        public int Count { get; set; }
        [JsonProperty("PracticePoint")]
        public double PracticePoint { get; set; }
        [JsonProperty("RankPoint")]
        public double RankPoint { get; set; }
    }
}
