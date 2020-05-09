using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class MailLogEntity : EntityBase
    {
        [JsonProperty("Sender")]
        public string Sender { get; set; }
        [JsonProperty("UID")]
        public string UID { get; set; }
        [JsonProperty("Type")]
        public int Type { get; set; }
        [JsonProperty("ActionType")]
        public int ActionType { get; set; }
        [JsonProperty("SendTime")]
        public DateTime SendTime { get; set; }
        [JsonProperty("Receiver")]
        public string Receiver { get; set; }
        [JsonProperty("Status")]
        public int Status { get; set; }
        [JsonProperty("OpenTime")]
        public DateTime OpenTime { get; set; }
        [JsonProperty("IP")]
        public string IP { get; set; }
    }

    public class MailStatus
    {
        public const int PENDING = 0, SENT = 1, OPENED = 2;
    }

    public class MailType
    {
        public const int INDIVIDUAL = 0, BULK = 1;
    }

    public class MailLogService : ServiceBase<MailLogEntity>
    {
        public MailLogService(IConfiguration config) : base(config)
        {

        }
    }
}
