using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace BaseEasyRealTime.Entities
{
    public class GroupEntity : EntityBase
    {
        public string DisplayName { get; set; }
        public string Name { get; set; } = new Guid().ToString();  // tên khởi tạo ban đầu , viết liền không dấu

        public string ParentID { get; set; } // = null / = Name
        public HashSet<MemberGroupInfo> Members { get; set; } = new HashSet<MemberGroupInfo>(); // thành viên
        public DateTime? Created { get; set; } // ngày tạo
        public string CreateUser { get; set; }
        public HashSet<MemberGroupInfo> MasterGroup { get; set; } = new HashSet<MemberGroupInfo>();
        public bool? Status { get; set; }// xóa bỏ // hoat động
        public bool? IsPrivateChat { get; set; } // user to user
    }
    public class GroupService : ServiceBase<GroupEntity>
    {
        public GroupService(IConfiguration config) : base(config)
        {

        }

        public GroupEntity CreateNewGroup(MemberGroupInfo sender, MemberGroupInfo receiver)
        {
            if (sender.ID == receiver.ID)
            {
                var listFilter = new List<FilterDefinition<GroupEntity>>()
                {
                    Builders<GroupEntity>.Filter.Eq(o=>o.IsPrivateChat,true),
                    Builders<GroupEntity>.Filter.AnyEq("Members",new HashSet<MemberGroupInfo>(){ sender}),
                };
                var item = CreateQuery().Find(Builders<GroupEntity>.Filter.And(listFilter))?.FirstOrDefault();
                if (item == null)
                {
                    item = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        CreateUser = sender.ID,
                        DisplayName = sender.Name,
                        Name = Guid.NewGuid().ToString(),
                        IsPrivateChat = true,
                        MasterGroup = new HashSet<MemberGroupInfo>() { sender },
                        Members = new HashSet<MemberGroupInfo>() { sender },
                        Status = true
                    };
                    CreateOrUpdate(item);
                    return item;
                }
                else
                {
                    return item;
                }
            }
            else
            {
                var listFilter = new List<FilterDefinition<GroupEntity>>()
                {
                    Builders<GroupEntity>.Filter.Eq(o=>o.IsPrivateChat,true),
                    Builders<GroupEntity>.Filter.Or(
                        Builders<GroupEntity>.Filter.AnyEq("Members",new HashSet<MemberGroupInfo>(){ receiver,sender}),
                        Builders<GroupEntity>.Filter.AnyEq("Members",new HashSet<MemberGroupInfo>(){ sender,receiver})),
                };
                var item = CreateQuery().Find(Builders<GroupEntity>.Filter.And(listFilter))?.FirstOrDefault();
                if (item == null)
                {
                    item = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        CreateUser = sender.ID,
                        DisplayName = "",
                        Name = Guid.NewGuid().ToString(),
                        IsPrivateChat = true,
                        MasterGroup = new HashSet<MemberGroupInfo>() { sender, receiver },
                        Members = new HashSet<MemberGroupInfo>() { sender, receiver },
                        Status = true
                    };
                    CreateOrUpdate(item);
                    return item;
                }
                else
                {
                    return item;
                }
            }
        }

        public long UpdateGroupDisplayName(string Name, string DisplayName)
        {
            var filter = Builders<GroupEntity>.Filter.Eq(t => t.Name, Name);
            var update = Builders<GroupEntity>.Update.Set(t => t.DisplayName, DisplayName);
            return Collection.UpdateMany(Builders<GroupEntity>.Filter.And(filter), update, new UpdateOptions { IsUpsert = false }).ModifiedCount;
        }

    }
    public class MemberGroupInfo
    {
        public MemberGroupInfo(string iD, string email, string name, bool isTeacher)
        {
            ID = iD;
            Email = email;
            Name = name;
            IsTeacher = isTeacher;
        }
        public string ID { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public bool IsTeacher { get; set; }
    }
}
