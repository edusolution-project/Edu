using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class MyCourseViewModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("Chapters")]
        public List<ChapterViewModel> Chapters { get; set; } = new List<ChapterViewModel>() { };
        [JsonProperty("Lessons")]
        public List<LessonViewModel> Lessons { get; set; } = new List<LessonViewModel>() { };
        
    }
    

}
