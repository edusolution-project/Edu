using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class StudentViewModel : StudentEntity
    {
        [JsonProperty("AccountID")]
        public string AccountID {get;set;}


    }
}
