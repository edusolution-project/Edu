using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class NewFeedEntity : EntityBase
    {
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("Title")]
        public string Title { get; set; }// nội dung
        [JsonProperty("Content")]
        public string Content { get; set; }// nội dung
        [JsonProperty("Medias")]
        public List<MediaEntity> Medias { get; set; } // docx, ảnh , video , audio , ...
        [JsonProperty("TimePost")]
        public DateTime TimePost { get; set; }
        [JsonProperty("Poster")]
        public string Poster { get; set; } // userid
        [JsonProperty("GroupID")]
        public string GroupID { get; set; }
        [JsonProperty("PosterName")]
        public string PosterName { get; set; } // tên người đăng
        [JsonProperty("Likes")]
        public List<string> Likes { get; set; }
        [JsonProperty("UnLikes")]
        public List<string> UnLikes { get; set; }
    }
    public class NewFeedService : ServiceBase<NewFeedEntity>
    {
        public NewFeedService(IConfiguration config) : base(config)
        {

        }
    }
}
