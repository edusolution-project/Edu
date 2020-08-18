using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseEasyRealTime.Entities
{
    public class MessagerEntity : EntityBase
    {
        [JsonProperty("s")]
        public string Sender { get; set; } // id
        [JsonProperty("r")]
        public string Receiver { get; set; } // id user / group 
        [JsonProperty("t")]
        public double Time { get; set; }
        [JsonProperty("d")]
        public MessageData Data { get; set; }
        [JsonProperty("edit")]
        public bool IsEdit { get; set; }
        [JsonProperty("del")]
        public bool IsDel { get; set; }
        [JsonProperty("u")]
        public double UpdateTime { get; set; }
        [JsonProperty("p")]
        public int State { get; set; } // 0 user-to-user / 1 user-to-group /2 hethong send all / 3 newfeed
        [JsonProperty("v")]
        public Viewer Views { get; set; }
        [JsonProperty("a")]
        public bool IsAll { get; set; } // all clear Views
    }
    public class Viewer
    {
        [JsonProperty("u")]
        public List<string> Users { get; set; }
    }
    public class MessageData
    {
        [JsonProperty("t")]
        public double TimeStamp { get; set; }
        [JsonProperty("c")]
        public string Text { get; set; }
        [JsonProperty("d")]
        public List<MetaData> MetaData { get; set; }
    }
    public class MetaData
    {
        [JsonProperty("t")]
        public double Time { get; set; }
        [JsonProperty("y")]
        public int Type { get; set; }// 0 ,1 ,2,3 
        [JsonProperty("c")]
        public string Text { get; set; }
        [JsonProperty("rl")]
        public DataRep Reply { get; set; }
        
    }
    public class DataRep
    {
        [JsonProperty("el")]
        public string El { get; set; } // id hoặc cái gì đó có thể đi tới el
        [JsonProperty("t")]
        public double Time { get; set; }
        [JsonProperty("s")]
        public string Sender { get; set; }
        [JsonProperty("c")]
        public string Content { get; set; } // noi dung html cua tin rep
    }
    public class MessagerService : ServiceBase<MessagerEntity>
    {
        public MessagerService(IConfiguration config) : base(config)
        {
            BsonClassMap.RegisterClassMap<MemberGroupInfo>();
            var indexs = new List<CreateIndexModel<MessagerEntity>>
            {
                new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t => t.Time)),
                new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t=> t.Receiver)),
                new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t=> t.State))
            };

            CreateQuery().Indexes.CreateManyAsync(indexs);
        }
    }
}
