using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Core_v2.Repositories;

namespace BaseCustomerMVC.Models
{
    public class PartViewModel: CloneLessonPartEntity
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("TemplateType")]
        public int TemplateType { get; set; }
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Questions")]
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>() { };
    }
}
