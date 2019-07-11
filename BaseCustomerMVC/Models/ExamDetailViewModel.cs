using BaseCustomerEntity.Database;
using Newtonsoft.Json;

namespace BaseCustomerMVC.Models
{
    public class ExamDetailViewModel:ExamDetailEntity
    {
        [JsonProperty("LessonScheduleName")]
        public string LessonScheduleName { get; set; }
        [JsonProperty("Question")]
        public string Question { get; set; }
        [JsonProperty("Answer")]
        public string Answer { get; set; }
        [JsonProperty("RealAnswer")]
        public string RealAnswer { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
    }
}
