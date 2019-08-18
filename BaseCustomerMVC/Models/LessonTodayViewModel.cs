using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class TodayClassViewModel : ClassEntity
    {
        [JsonProperty("Lessons")]
        public List<LessonScheduleTodayViewModel> Lessons { get; set; } = new List<LessonScheduleTodayViewModel>() { };

    }
    public class LessonScheduleTodayViewModel : StudentLessonViewModel
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
    }
}
