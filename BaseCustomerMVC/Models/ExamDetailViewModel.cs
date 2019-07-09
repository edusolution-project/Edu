using BaseCustomerEntity.Database;
using Newtonsoft.Json;

namespace BaseCustomerMVC.Models
{
    public class ExamDetailViewModel:ExamDetailEntity
    {
        [JsonProperty("LessonScheduleName")]
        public string LessonScheduleName { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
    }
}
