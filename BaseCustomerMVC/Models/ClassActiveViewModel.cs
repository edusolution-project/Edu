using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassActiveViewModel : ClassEntity
    {

        [JsonProperty("Progress")]
        public int Progress { get; set; }
    }
}
