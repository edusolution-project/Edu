﻿using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class NewsViewModel:NewsEntity
    {
        [JsonProperty("CategoryName")]
        public List<NewsCategoryEntity> CategoryName { get; set; }
        [JsonProperty("CenterName")]
        public string CenterName { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("TotalPrice")]
        public double TotalPrice { get; set; }
        [JsonProperty("Students")]
        public double Students { get; set; }
    }
}
