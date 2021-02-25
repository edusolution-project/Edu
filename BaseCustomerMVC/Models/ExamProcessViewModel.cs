using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ExamProcessViewModel
    {
        [JsonProperty("ExamQuestionArchiveID")] //ma kho de
        public String ExamQuestionArchiveID { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        [JsonProperty("StartTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty("EndTime")]
        public DateTime EndTime { get; set; }
        [JsonProperty("EasyQuestion")]
        public Int32 EasyQuestion { get; set; }
        [JsonProperty("NormalQuestion")]
        public Int32 NormalQuestion { get; set; }
        [JsonProperty("HardQuestion")]
        public Int32 HardQuestion { get; set; }
        [JsonProperty("Timer")] // thời gian làm bài
        public Int32 Timer { get; set; }
        [JsonProperty("Limit")]
        public Int32 Limit { get; set; }
        [JsonProperty("Etype")]
        public Int32 Etype { get; set; }
        [JsonProperty("Multiple")]
        public Int32 Multiple { get; set; }
        [JsonProperty("TargetClasses")]
        public List<String> TargetClasses { get; set; }
        [JsonProperty("TotalExam")]
        public Int32 TotalExam { get; set; } //so de muon tao
        [JsonProperty("LessonID")]
        public String LessonID { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }
    }
}
