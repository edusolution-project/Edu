using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace BaseEasyRealTime.Entities
{
    public class MessageEntity : EntityBase
    {
        public string Sender { get; set; }
        public HashSet<string> Receivers { get; set; } = new HashSet<string>();
        public string Content { get; set; }
        public List<FileManagerCore.Globals.MediaResponseModel> Medias { get; set; } = new List<FileManagerCore.Globals.MediaResponseModel>();
        public int State { get; set; } = 0;
        public HashSet<string> Views { get; set; }
        public bool RemoveByAdmin { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;

    }
    public class MessageService : ServiceBase<MessageEntity>
    {
        public MessageService(IConfiguration config) : base(config)
        {
        }
    }
}
