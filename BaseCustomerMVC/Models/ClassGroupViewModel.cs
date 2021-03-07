using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassGroupViewModel :ClassGroupEntity
    {
        [JsonProperty("Members")]
        public List<string> Members { get; set; }
        [JsonProperty("StudentCount")]
        public long StudentCount { get; set; }
    }
}
