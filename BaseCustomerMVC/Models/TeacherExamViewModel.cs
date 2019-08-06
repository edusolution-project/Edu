using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseCustomerMVC.Models
{
    public class TeacherExamViewModel : LessonEntity
    {
        [JsonProperty("Parts")]
        public List<ExamPartViewModel> Parts { get; set; } = new List<ExamPartViewModel>() { };
    }
}
