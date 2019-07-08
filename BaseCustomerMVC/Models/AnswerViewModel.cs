using System;
using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using Core_v2.Repositories;

namespace BaseCustomerMVC.Models
{
    public class AnswerViewModel:EntityBase
    {
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Media")]
        public Media Media { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }
    }
}
