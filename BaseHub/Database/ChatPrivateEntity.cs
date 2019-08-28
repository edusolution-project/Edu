using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseHub.Database
{
    public class ChatPrivateEntity : EntityBase
    {
        [JsonProperty("Receiver")]
        public string Receiver { get; set; }
        [JsonProperty("Sender")]
        public string Sender { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }
        [JsonProperty("Medias")]
        public List<string> Medias { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }

    }
    public class ChatPrivateService : ServiceBase<ChatPrivateEntity>
    {
        public ChatPrivateService(IConfiguration config) : base(config)
        {

        }
    }
}
