using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class Media
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("OriginalName")]
        public string OriginalName { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Extension")]
        public string Extension { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Size")]
        public double Size { get; set; }
    }
}
