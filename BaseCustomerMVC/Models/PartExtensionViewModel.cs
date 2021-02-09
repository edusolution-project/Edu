using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class PartExtensionViewModel :  CloneLessonPartExtensionEntity
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
        public List<QuestionExtensionViewModel> Questions { get; set; } = new List<QuestionExtensionViewModel>() { };
    }
}
