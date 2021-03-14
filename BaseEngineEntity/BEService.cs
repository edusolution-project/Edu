using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseEngineEntity
{
    public class ChatService : ServiceBase<ChatEntity>
    {
        private readonly ChatDetailService _chatDetailService;
        public ChatService(IConfiguration configuration, ChatDetailService chatDetailService) : base(configuration)
        {
            _chatDetailService = chatDetailService;
        }

        public async Task<ChatDetailEntity> GetDetail(string id)
        {
            var data = await _chatDetailService.CreateQuery().FindAsync(o => o.ChatID == id);
            return data?.SingleOrDefault();
        }

        public async Task<bool> UpdateAttachMements(string user,string chatId,List<Attachment> attachments)
        {

            if (string.IsNullOrEmpty(user) || attachments == null || attachments.Count == 0 || string.IsNullOrEmpty(chatId))
            {
                return false;
            }
            var detail = await _chatDetailService.Collection.FindAsync(o => o.ChatID == chatId);
            var item = detail == null ? null : detail?.SingleOrDefault();
            if (item == null)
            {
                return false;
            }
            item.Attachments.AddRange(attachments);
            await _chatDetailService.Collection.ReplaceOneAsync(o => o.ID == item.ID, item);
            return true;
        }

        public async Task<ChatEntity> GetSupport(string user)
        {
            var data = await CreateQuery().FindAsync(o => o.Type == ECHAT.SUPPORT && o.Admins.Contains(user));
            var item = data?.SingleOrDefault();
            if(item == null)
            {
                item = new ChatEntity()
                {
                   Admins = new List<string>() { user, "SUPPORT"},
                   Title = "Hỗ trợ khách hàng",
                   Type = ECHAT.SUPPORT
                };
                await CreateQuery().InsertOneAsync(item);
                var detail = new ChatDetailEntity()
                {
                    ChatID = item.ID,
                    Members = new List<Member>(){new Member(){ID = user},new Member(){ID = "SUPPORT"}}
                };
                await _chatDetailService.CreateQuery().InsertOneAsync(detail);
            }
            return item;
        }

        public async Task<List<ChatEntity>> GetList(string user, List<ChatEntity> chatId)
        {
            if (string.IsNullOrEmpty(user)){return null;}
            List<string> Ids = chatId.Select(x => x.ID)?.ToList();
            long count = await CreateQuery().CountDocumentsAsync(o => Ids.Contains(o.ID));
            if(count == 0 || count < chatId.Count)
            {
                CreateChat
            }
            //FilterDefinition<ChatEntity> filter
            //var filter = Builders<ChatEntity>.Filter.AnyEq(o=>o.Admins,)
            var data = await CreateQuery().FindAsync(o => 
            (o.Admins.Contains(user) && o.Type == ECHAT.USER) || // user to user
            (chatId != null && chatId.Count > 0 && Ids.Contains(o.ID)) || // group to user
            o.Type == ECHAT.SUPPORT || // support
            o.Type == ECHAT.SYSTEM);   // he thong
            return data?.ToList();
        }

        private bool Exist(string id)
        {
            return GetItemByID(id) != null;
        }

        public async Task<bool> UpdateLastRead(string userId,string id, string messageId)
        {
            if (Exist(id))
            {
                var data = await _chatDetailService.CreateQuery().FindAsync(o => o.ChatID == id);
                var item = data.SingleOrDefault();
                var member = item.Members.SingleOrDefault(o => o.ID == userId);
                member.LastRead = messageId;
                _chatDetailService.CreateOrUpdate(item);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateLastMessage (string id,MessageEntity message)
        {
            var data = GetItemByID(id);
            if (data != null)
            {
                data.LastMessage = message;
                await CreateQuery().ReplaceOneAsync(o => o.ID == id, data);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateChat(ChatEntity item)
        {
            var update = await CreateQuery().ReplaceOneAsync(o => o.ID == item.ID, item);
            return update.IsModifiedCountAvailable;
        }

        public async Task<bool> KickMember(string id, string memberId)
        {
            if (Exist(id))
            {
                var data = await _chatDetailService.CreateQuery().FindAsync(o => o.ChatID == id);
                var item = data.SingleOrDefault();
                var member = item.Members.SingleOrDefault(o => o.ID == memberId);
                if (member != null)
                {
                    item.Members.Remove(member);
                }
                _chatDetailService.CreateOrUpdate(item);
                return true;
            }
            return false;
        }
        public async Task<bool> AddMembers(string id, string memberId)
        {
            if (Exist(id))
            {
                var data =  await _chatDetailService.CreateQuery().FindAsync(o => o.ChatID == id);
                var item = data.SingleOrDefault();
                item.Members.Add(new Member() { ID = memberId });
                _chatDetailService.CreateOrUpdate(item);
                return true;
            }
            return false;
        }
        public async Task CreatePrivate(string sender, string friend)
        {
            var data = await CreateQuery().FindAsync(o => o.Admins.Contains(sender) && o.Admins.Contains(friend) && o.Type == ECHAT.USER);
            if(data?.SingleOrDefault() == null)
            {
                ChatEntity chat = new ChatEntity()
                {
                    Admins = new List<string>() { sender,friend },
                    Type = ECHAT.USER
                };
                await CreateQuery().InsertOneAsync(chat);
                ChatDetailEntity detail = new ChatDetailEntity()
                {
                    ChatID = chat.ID,
                    Members = new List<Member>() {new Member(){ID = sender},new Member(){ID = friend},}
                };
                await _chatDetailService.CreateQuery().InsertOneAsync(detail);
            }
        }

        public async Task<bool> CreateGroup(string id, string title,List<string> admins, List<Member> members, ECHAT type = ECHAT.GROUP)
        {
            // khong ton tai
            if (!Exist(id))
            {
                ChatEntity chat = new ChatEntity()
                {
                    ID = id,
                    Title = title,
                    Admins = admins,
                    Type = type
                };
                ChatDetailEntity detail = new ChatDetailEntity()
                {
                    ChatID = id,
                    Members = members
                };

                await CreateQuery().InsertOneAsync(chat);
                await _chatDetailService.CreateQuery().InsertOneAsync(detail);
                return true;
            }
            return false;
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
        private readonly ChatService _chatService;
        public MessageService(IConfiguration configuration, ChatService chatService) : base(configuration)
        {
            _chatService = chatService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user">current user</param>
        /// <param name="receiver">chat id  /  user id</param>
        /// <param name="time"></param>
        /// <param name="type"></param>
        /// <param name="limit">20</param>
        /// <param name="skip">0=>199</param>
        /// <returns></returns>
        public async Task<List<MessageEntity>> GetList(string user,string chatId,ECHAT chatType, double time = 0, int limit = 20, int skip = 0)
        {
            if (time == 0) time = new UnixTime().Now;
            var options = new FindOptions<MessageEntity, MessageEntity>()
            {
                Limit = limit,
                Skip = skip*limit,
                Sort = Builders<MessageEntity>.Sort.Ascending(o => o.Created)
            };
            var chat = chatType == ECHAT.SUPPORT ? await _chatService.GetSupport(user) : _chatService.GetItemByID(chatId);
            if(chat == null)
            {
                return null;
            }

            var detail = await _chatService.GetDetail(chat.ID);
            if(detail.Members.SingleOrDefault(o=>o.ID == user) == null)
            {
                return null;
            }

            FilterDefinition<MessageEntity> builderFilter;
            switch (chat.Type)
            {
                case ECHAT.GROUP:
                    builderFilter = Builders<MessageEntity>.Filter.And(
                        Builders<MessageEntity>.Filter.Eq(o => o.Receiver, chat.ID),
                        Builders<MessageEntity>.Filter.Eq(o => o.Type, EMessageType.GROUP),
                        Builders<MessageEntity>.Filter.Where(o=> o.Created <= time));
                    break;
                case ECHAT.SUPPORT:
                    builderFilter = Builders<MessageEntity>.Filter.And(
                        Builders<MessageEntity>.Filter.Eq(o => o.Receiver, chat.ID), 
                        Builders<MessageEntity>.Filter.Eq(o => o.Type, EMessageType.SUPPORT),
                        Builders<MessageEntity>.Filter.Where(o => o.Created <= time));
                    break;
                case ECHAT.SYSTEM:
                    builderFilter = Builders<MessageEntity>.Filter.And(Builders<MessageEntity>.Filter.Eq(o => o.Type, EMessageType.SYSTEM));
                    break;
                case ECHAT.USER:
                    builderFilter = Builders<MessageEntity>.Filter.And(
                        Builders<MessageEntity>.Filter.Eq(o => o.Receiver, chat.ID), 
                        Builders<MessageEntity>.Filter.Eq(o => o.Type, EMessageType.USER),
                        Builders<MessageEntity>.Filter.Where(o => o.Created <= time));
                    break;
                default:
                    return null;
            }
            long count = await CreateQuery().CountDocumentsAsync(builderFilter);
            if (count > limit * (skip + 1))
            {
                var data = await CreateQuery().FindAsync(builderFilter);
                return data?.ToList();
            }
            else
            {
                var data = await CreateQuery().FindAsync(builderFilter, options);
                return data?.ToList();
            }
            
        }
        public async Task<string> Create(MessageEntity item)
        {
            await CreateQuery().InsertOneAsync(item);
            return item.ID;
        }
        public async Task<bool> Update(MessageEntity item)
        {
            var oldItem = GetItemByID(item.ID);
            if (oldItem == null) return false;
            var result = await CreateQuery().ReplaceOneAsync(o => o.ID == oldItem.ID, item);
            return true;
        }
        public async Task<bool> Remove(string messageId, string user)
        {
            var delete = await CreateQuery().FindOneAndDeleteAsync(o => o.ID == messageId && o.Sender.ID == user);
            return delete != null;
        } 
    }
}
