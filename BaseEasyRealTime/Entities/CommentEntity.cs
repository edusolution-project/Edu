using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BaseEasyRealTime.Entities
{
    public class CommentEntity : EntityBase
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public List<FileManagerCore.Globals.MediaResponseModel> Medias { get; set; } = new List<FileManagerCore.Globals.MediaResponseModel>();
        public string ParentID { get; set; } // new feed id
        public bool IsReply { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

    }
    public class CommentService : ServiceBase<CommentEntity>
    {
        public CommentService(IConfiguration config) : base(config)
        {
        }
    }
}
