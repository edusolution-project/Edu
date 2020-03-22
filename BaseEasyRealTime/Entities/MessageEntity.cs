using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using MongoDB.Driver;

namespace BaseEasyRealTime.Entities
{
    public class MessageEntity : EntityBase
    {
        public string Code { get; set; } = Guid.NewGuid().ToString();
        public object Sender { get; set; }
        public HashSet<string> Receivers { get; set; } = new HashSet<string>(); // chir dung cho new feed
        public string Receiver { get; set; } // groupName
        public string Title { get; set; }
        public string Content { get; set; }
        public List<FileManagerCore.Globals.MediaResponseModel> Medias { get; set; } = new List<FileManagerCore.Globals.MediaResponseModel>();
        public int State { get; set; } = 0;
        public HashSet<string> Views { get; set; }
        public string ReplyTo { get; set; } // code message
        public bool? RemoveByAdmin { get; set; } = false;
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Updated { get; set; } = DateTime.Now;

    }
    public class MessageService : ServiceBase<MessageEntity>
    {
        [Obsolete]
        public MessageService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<MessageEntity>>
            {
                //CourseID_1
                new CreateIndexModel<MessageEntity>(
                    new IndexKeysDefinitionBuilder<MessageEntity>()
                    .Ascending(t => t.Code)),
                //ParentID_1
                new CreateIndexModel<MessageEntity>(
                    new IndexKeysDefinitionBuilder<MessageEntity>()
                    .Ascending(t=> t.Receiver)),
                new CreateIndexModel<MessageEntity>(
                    new IndexKeysDefinitionBuilder<MessageEntity>()
                    .Ascending(t=> t.State))
            };

            CreateQuery().Indexes.CreateManyAsync(indexs);
        }

        public MessageEntity CreateMessage(MessageEntity item)
        {
            CreateOrUpdate(item);
            return item;
        }
        public MessageEntity GetItemByCode(string Code)
        {
            if (string.IsNullOrEmpty(Code)) return null;
            return CreateQuery().FindAsync(Builders<MessageEntity>.Filter.Eq(o => o.Code, Code)).Result?.FirstOrDefault();
        }

        public IEnumerable<MessageEntity> GetMessageList(string GroupName, DateTime StartDate, DateTime EndDate)
        {
            var data = CreateQuery().Find(o =>o.State == 0 && ((o.Receiver == GroupName && o.Created >= StartDate)||(o.Receiver == GroupName && o.Created <= EndDate))
            )?.ToList();

            if (data == null || data.Count == 0)
            {
                var itemLast = CreateQuery().Find(o => o.State == 0 && o.Receiver == GroupName)?.SortByDescending(o => o.Created)?.Limit(1)?.FirstOrDefault();
                if (itemLast != null)
                {
                    EndDate = itemLast.Created.Value;
                    StartDate = EndDate.AddDays(-7);
                    data = CreateQuery().Find(o => o.State == 0 && ((o.Receiver == GroupName && o.Created >= StartDate) || (o.Receiver == GroupName && o.Created <= EndDate)))?.ToList();
                }
                else
                {
                    return null;
                }
            }

            return data;
        }
        public IEnumerable<MessageEntity> GetNewFeedList(string GroupName, DateTime StartDate, DateTime EndDate)
        {
            var data = CreateQuery().Find(o =>
            o.State == 1 &&
            ((o.Receiver == GroupName && o.Created >= StartDate)
            ||
            (o.Receiver == GroupName && o.Created <= EndDate))
            )?.ToList();

            if (data == null || data.Count == 0) {
                var itemLast = CreateQuery().Find(o => o.State == 1 && o.Receiver == GroupName)?.SortByDescending(o => o.Created)?.Limit(1)?.FirstOrDefault();
                if (itemLast != null)
                {
                    EndDate = itemLast.Created.Value;
                    StartDate = EndDate.AddDays(-7);
                    data = CreateQuery().Find(o =>o.State == 1 &&((o.Receiver == GroupName && o.Created >= StartDate)||(o.Receiver == GroupName && o.Created <= EndDate)))?.ToList();
                }
                else
                {
                    return null;
                }
            }

            return data;
        }
    }
}
