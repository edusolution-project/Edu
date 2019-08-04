using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class ChatEntity : EntityBase
    {
        [JsonProperty("Sender")]
        public string Sender { get; set; }
        [JsonProperty("Reciever")]
        public string Reciever { get; set; }
        [JsonProperty("IsGroup")]
        public bool IsGroup { get; set; } // nếu là group reciever => là group name
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Medias")]
        public List<MediaModel> Medias { get; set; }
        [JsonProperty("Time")]
        public DateTime Time { get; set; }
        [JsonProperty("UserView")]
        public string UserView { get; set; }
        [JsonProperty("Tags")]
        public List<string> Tags { get; set; }
    }
    public class ChatService : ServiceBase<ChatEntity>
    {
        public ChatService(IConfiguration config) : base(config)
        {

        }
    }
}
