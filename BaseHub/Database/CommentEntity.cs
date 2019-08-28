using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class CommentEntity : EntityBase
    {
        [JsonProperty("Content")]
        public string Content { get; set; }// nội dung
        [JsonProperty("Medias")]
        public List<MediaEntity> Medias { get; set; } // docx, ảnh , video , audio , ...
        [JsonProperty("TimePost")]
        public DateTime TimePost { get; set; }
        [JsonProperty("Poster")]
        public string Poster { get; set; } // userid
        [JsonProperty("NewFeedID")]
        public string NewFeedID { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("PosterName")]
        public string PosterName { get; set; } // tên người đăng
        [JsonProperty("Likes")]
        public List<string> Likes { get; set; }
        [JsonProperty("UnLikes")]
        public List<string> UnLikes { get; set; }
    }
    public class CommentService : ServiceBase<CommentEntity>
    {
        public CommentService(IConfiguration config) : base(config)
        {

        }
    }
}
