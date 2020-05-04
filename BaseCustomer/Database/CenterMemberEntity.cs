using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class CenterMemberEntity
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Type")]
        public int Type { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
    }

    public class CenterMemberType
    {
        public const int
            TEACHER = 1,
            MANAGER = 4,
            OWNER = 5;
    }

}
