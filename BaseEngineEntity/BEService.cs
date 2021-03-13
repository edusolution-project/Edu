using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseEngineEntity
{
    public class ChatService : ServiceBase<ChatEntity>
    {
        public ChatService(IConfiguration configuration) : base(configuration)
        {

        }
    }
    public class ChatDetailService : ServiceBase<ChatDetailEntity>
    {
        public ChatDetailService(IConfiguration configuration) : base(configuration)
        {

        }
    }
    public class MessageService : ServiceBase<MessageEntity>
    {
        public MessageService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
