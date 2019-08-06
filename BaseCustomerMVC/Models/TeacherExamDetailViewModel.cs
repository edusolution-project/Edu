using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseCustomerMVC.Models
{
    public class TeacherExamDetailViewModel:ExamDetailEntity
    {
        [JsonProperty("LessonScheduleName")]
        public string LessonScheduleName { get; set; }
        [JsonProperty("Question")]
        public string Question { get; set; }
        //[JsonProperty("AnswerID")]
        //public string AnswerID { get; set; }
        //[JsonProperty("AnswerValue")]
        //public string AnswerValue { get; set; }
        //[JsonProperty("RealAnswerID")]
        //public string RealAnswerID { get; set; }
        //[JsonProperty("RealAnswerValue")]
        //public string RealAnswerValue { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("CorrectAnswerOrg")]
        public List<LessonPartAnswerEntity> CorrectAnswerOrg { get; set; }
        [JsonProperty("CorrectAnswer")]
        public List<CloneLessonPartAnswerEntity> CorrectAnswer { get; set; }
        //[JsonProperty("Point")]
        //public double Point{get;set;}
        [JsonProperty("MaxPoint")]
        public double MaxPoint{get;set;}
    }
}
