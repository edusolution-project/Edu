using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class MarkViewModel 
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Multiple")]
        public double Multiple { get; set; }
        [JsonProperty("Type")]
        public int Type { get; set; }
    }
}
