using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    [Serializable]
    public class ClassResultViewModel
    {
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
        //[JsonProperty("tcids")]
        //public List<ClassMemberEntity> tcids { get; set; }
        [JsonProperty("Above8")]
        public double Above8 { get; set; }
        [JsonProperty("Above5")]
        public double Above5 { get; set; }
        [JsonProperty("Above2")]
        public double Above2 { get; set; }
        [JsonProperty("Above0")]
        public double Above0 { get; set; }
        [JsonProperty("Attend")]
        public double Attend { get; set; }
    }
}
