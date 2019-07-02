using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class QuestionViewModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }

        [JsonProperty("Answers")]
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>() { };
    }
}
