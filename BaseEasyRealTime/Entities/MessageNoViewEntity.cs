using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseEasyRealTime.Entities
{
    public class MessageNoViewEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public List<MemberGroupInfo> Viewer { get; set; }
    }
    public class MessageNoViewSerivce : ServiceBase<MessageNoViewEntity>
    {
        public MessageNoViewSerivce(IConfiguration config) : base(config)
        {
        }
    }
}
