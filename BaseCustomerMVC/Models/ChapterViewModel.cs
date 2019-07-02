using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class ChapterViewModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Lessons")]
        public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>() { };
    }
}
