using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class TeacherViewModel : TeacherEntity
    {
        [JsonProperty("SubjectList")]
        public List<SubjectEntity> SubjectList { get; set; }
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("RoleName")]
        public string RoleName { get; set; }
        [JsonProperty("AccountID")]
        public string AccountID {get;set;}


    }
}
