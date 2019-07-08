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
    }
}
