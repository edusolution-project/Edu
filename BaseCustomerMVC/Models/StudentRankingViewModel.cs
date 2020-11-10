using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    [Serializable]
    public class StudentRankingViewModel
    {
        [JsonProperty("Rank")]
        public int Rank { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("PracticeAvgPoint")]
        public double PracticeAvgPoint { get; set; }
        [JsonProperty("TotalPoint")]
        public double TotalPoint { get; set; }
        [JsonProperty("ExamDone")]
        public long ExamDone { get; set; }
        [JsonProperty("PracticeDone")]
        public long PracticeDone { get; set; }
        [JsonProperty("Count")]
        public int Count { get; set; }
        [JsonProperty("PracticePoint")]
        public double PracticePoint { get; set; }
        [JsonProperty("RankPoint")]
        public double RankPoint { get; set; }
        [JsonProperty("LastDate")]
        public DateTime LastDate { get; set; }
    }
}
