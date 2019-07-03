using System;
using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseCustomerMVC.Models
{
    public class AnswerViewModel
    {
        [JsonProperty("ID")]
        public string ID { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
}
