using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassMemberViewModel : StudentEntity
    {
        
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("LastJoinDate")]
        public DateTime LastJoinDate { get; set; }
        [JsonProperty("ClassStatus")]
        public string ClassStatus { get; set; }
        [JsonProperty("LastJoin")]
        public DateTime LastJoin { get; set; } 
        [JsonProperty("Progress")]
        public int Progress { get; set; } 
    }
}
