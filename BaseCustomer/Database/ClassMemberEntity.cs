using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class ClassMemberEntity
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Type")]
        public int Type { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
    }

    public class ClassMemberType
    {
        public const int 
            TEACHER = 1,
            CLASS_ASSISTANT = 2,
            CUSTOMER_MANAGEMENT = 3;
    }

}
