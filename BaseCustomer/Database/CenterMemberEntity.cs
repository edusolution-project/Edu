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
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("CenterID")]
        public string CenterID { get; set; }
    }

}
