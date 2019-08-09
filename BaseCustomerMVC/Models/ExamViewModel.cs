using BaseCustomerEntity.Database;
using Newtonsoft.Json;

namespace BaseCustomerMVC.Models
{
    public class ExamViewModel : ExamEntity
    {
        [JsonProperty("LessonScheduleName")]
        public string LessonScheduleName { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
    }
}
