using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyChatApp.DataBase
{
    public class GroupUserEntity : EntityBase
    {
        public string Name { get; set; } = Guid.NewGuid().ToString(); // ten tu sinh
        public string DisplayName { get; set; }
        public List<string> Members { get; set; } = new List<string>();
        public List<string> Ours { get; set; } // chu so huu
        public string CreateUser { get; set; } // nguoi tao
        public double TimeCreated { get; set; }
        public bool IsPrivate { get; set; } = true; // user to user
    }
    public class MessagerEntity : EntityBase
    {
        public string Content { get; set; } // noi dung
        public List<MetaData> Data { get; set; } // url, 
        public string Sender { get; set; } // nguoi gui
        public string GroupId { get; set; } // ten group
        public double Time { get; set; } = new UnixTime().Now(); // gio gui
        public bool IsDel { get; set; } = false;
        public bool IsPublic { get; set; } = false;
    }
    public class MetaData
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Id { get; set; }
    }
    public class MessagerService : ServiceBase<MessagerEntity>
    {
        public MessagerService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<MessagerEntity>>
            {
                new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t => t.Time)),
                 new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t => t.IsPublic)),
                new CreateIndexModel<MessagerEntity>(
                    new IndexKeysDefinitionBuilder<MessagerEntity>()
                    .Ascending(t=> t.GroupId))
            };

            CreateQuery().Indexes.CreateManyAsync(indexs);
        }
    }
    public class GroupUserService : ServiceBase<GroupUserEntity>
    {
        private readonly UnixTime _unixTime;
        public GroupUserService(IConfiguration config) : base(config)
        {
            if (_unixTime == null)
            {
                _unixTime = new UnixTime();
            }
        }

        public GroupUserEntity CreatePrivate(string displayName, string sender, string member)
        {
            var oldItem = CreateQuery().Find(o => o.IsPrivate == true && o.Members.Any(x => x == sender || x == member))?.SingleOrDefault();
            if (oldItem == null)
            {
                var item = new GroupUserEntity()
                {
                    Name = $"{sender}_{member}",
                    CreateUser = sender,
                    DisplayName = displayName,
                    IsPrivate = true,
                    TimeCreated = _unixTime.Now(),
                    Members = new List<string>() { sender, member }
                };
                CreateQuery().InsertOne(item);
                return item;
            }
            else
            {
                oldItem.DisplayName = displayName;
                CreateQuery().ReplaceOne(o => o.ID == oldItem.ID, oldItem);
                return oldItem;
            }
        }
        public GroupUserEntity GetGroupPrivate(string member1, string member2)
        {
            string strName1 = $"{member1}_{member2}";
            string strName2 = $"{member2}_{member1}";
            var oldItem = CreateQuery().Find(o => o.IsPrivate == true && (o.Name == strName1 || o.Name == strName2))?.SingleOrDefault();
            if(oldItem == null)
            {
                oldItem = CreatePrivate(strName1, member1, member2);
            }

            return new GroupUserEntity
            {
                ID = oldItem.ID,
                Name = oldItem.Name,
                Members = oldItem.Members,
                DisplayName = oldItem.DisplayName
            };
        }
    }
    public class UnixTime
    {
        public UnixTime()
        {

        }

        public double Now()
        {
            return (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
        public double Date(DateTime dateTime)
        {
            return (dateTime.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
        }
    }
}
