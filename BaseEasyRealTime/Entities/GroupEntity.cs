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
        public HashSet<string> Members { get; set; } = new HashSet<string>(); // thành viên
        public DateTime Created { get; set; } // ngày tạo
        public string CreateUser { get; set; }
        public HashSet<string> MasterGroup { get; set; } = new HashSet<string>();
        public bool Status { get; set; } = true;// xóa bỏ // hoat động
        public bool IsPrivateChat { get; set; } = false; // user to user
    }
    public class GroupService : ServiceBase<GroupEntity>
    {
        public GroupService(IConfiguration config) : base(config)
        {

        }
        public string GetGroupName(string member1, string member2)
        {
            var item = Collection.Find(o => o.IsPrivateChat == true && o.Members.Count == 2 && o.Members.Contains(member1) && o.Members.Contains(member2))?.SingleOrDefault();
            if(item == null)
            {
                item = new GroupEntity()
                {
                    Created = DateTime.Now,
                    CreateUser = member1,
                    DisplayName = "",
                    Members = new HashSet<string>() { member1, member2 },
                    Name = new Guid().ToString(),
                    Status = true,
                    IsPrivateChat = true
                };
                Collection.InsertOne(item);
            }
            return item.Name;
        }
        public GroupEntity Create(string displayName,string name, string userCreated, HashSet<string> memembers, HashSet<string> masterGroup)
        {
            var item = new GroupEntity()
            {
                Created = DateTime.Now,
                CreateUser = userCreated,
                DisplayName = displayName,
                MasterGroup = masterGroup,
                Members = memembers,
                Name = name,
                Status = true
            };
            Collection.InsertOne(item);
            return item;
        }
        public GroupEntity ChangeDisplayName(string id,string displayName)
        {
            var item = GetItemByID(id);
            if(item != null)
            {
                item.DisplayName = displayName;
                CreateOrUpdate(item);
            }
            return item;
        }

    }
}
