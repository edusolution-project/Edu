using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class MediaEntity
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Host")]
        public string Host { get; set; }
        [JsonProperty("Extends")]
        public string Extends { get; set; }
    }
}
