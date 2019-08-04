using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class CommentEntity : EntityBase
    {
        [JsonProperty("NewID")]
        public string NewID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("Sender")]
        public string Sender { get; set; }
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Medias")]
        public List<MediaModel> Medias { get; set; }
        [JsonProperty("Likes")]
        public List<string> Likes { get; set; }
        [JsonProperty("Tags")]
        public List<string> Tags { get; set; }

    }
}
