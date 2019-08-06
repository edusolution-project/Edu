using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Core_v2.Repositories;

namespace BaseCustomerMVC.Models
{
    public class ExamPartViewModel: CloneLessonPartEntity
    {
        [JsonProperty("ExamDetails")]
        public List<TeacherExamDetailViewModel> ExamDetails { get; set; } = new List<TeacherExamDetailViewModel>() { };
    }
}
