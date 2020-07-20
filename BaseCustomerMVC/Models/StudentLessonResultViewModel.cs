using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class StudentLessonResultViewModel
    {

        public StudentLessonResultViewModel(StudentEntity student)
        {
            StudentID = student.ID;
            StudentName = student.FullName;
        }

        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("MaxPoint")]
        public double MaxPoint { get; set; }
        [JsonProperty("MinPoint")]
        public double MinPoint { get; set; }
        [JsonProperty("AvgPoint")]
        public double AvgPoint { get; set; }
        [JsonProperty("LastPoint")]
        public double LastPoint { get; set; }
        [JsonProperty("LastOpen")]
        public string LastOpen { get; set; }
        [JsonProperty("TriedCount")]
        public double TriedCount { get; set; }
        [JsonProperty("LastTried")]
        public DateTime LastTried { get; set; }
        [JsonProperty("IsCompleted")]
        public bool IsCompleted { get; set; }
        [JsonProperty("ListExam")]
        public List<ExamDetailCompactView> ListExam { get; set; }
    }

    public class ExamDetailCompactView
    {
        public ExamDetailCompactView(ExamEntity t)
        {
            ExamID = t.ID;
            Created = t.Created;
            Marked = t.Marked;
            QuestionsDone = t.QuestionsDone;
            QuestionsPass = t.QuestionsPass;
            QuestionsTotal = t.QuestionsTotal;
            Point = t.QuestionsTotal > 0 ? t.QuestionsPass * 100 / t.QuestionsTotal : 0;
            Status = t.Status;
            Marked = t.Marked;
        }

        [JsonProperty("ExamID")]
        public string ExamID { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Marked")]
        public bool Marked { get; set; }
        [JsonProperty("QuestionsTotal")]
        public long QuestionsTotal { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("QuestionsDone")]
        public long QuestionsDone { get; set; }
        [JsonProperty("QuestionsPass")]
        public long QuestionsPass { get; set; }
        [JsonProperty("Status")]
        public bool Status { get; set; }
    }


}
