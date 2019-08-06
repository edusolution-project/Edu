using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class MediaModel
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Extension")]
        public string Extension { get; set; }
    }
}
