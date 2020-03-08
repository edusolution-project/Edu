﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseEasyRealTime.Entities
{
    public class GroupEntity : EntityBase
    {
        public string DisplayName { get; set; }
        public string Name { get; set; } = new Guid().ToString();  // tên khởi tạo ban đầu , viết liền không dấu

        public string ParentID { get; set; } // = null / = Name
        public HashSet<MemberGroupInfo> Members { get; set; } = new HashSet<MemberGroupInfo>(); // thành viên
        public DateTime Created { get; set; } // ngày tạo
        public string CreateUser { get; set; }
        public HashSet<MemberGroupInfo> MasterGroup { get; set; } = new HashSet<MemberGroupInfo>();
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
            var dataID = Guid.NewGuid().ToString();
            if (member1 == member2)
            {
                var listItem = Collection.Find(o => o.IsPrivateChat == true && o.Members.Count == 1 && o.Members.Contains(member1))?.ToList();
                var item = listItem == null || listItem.Count == 0 ? null : listItem?.LastOrDefault();
                if (item == null)
                {
                    item = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        CreateUser = member1,
                        DisplayName = "",
                        Members = new HashSet<string>() { member1},
                        Name = dataID,
                        Status = true,
                        IsPrivateChat = true
                    };
                    Collection.InsertOne(item);
                }
                return item.Name;
            }
            else
            {
                var listItem = Collection.Find(o => o.IsPrivateChat == true && o.Members.Count == 2 && (o.Members.Contains(member1) && o.Members.Contains(member2)))?.ToList();
                var item = listItem == null || listItem.Count == 0 ? null : listItem?.LastOrDefault();
                if (item == null)
                {
                    item = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        CreateUser = member1,
                        DisplayName = "",
                        Members = new HashSet<string>() { member1, member2 },
                        Name = dataID,
                        Status = true,
                        IsPrivateChat = true
                    };
                    Collection.InsertOne(item);
                }
                return item.Name;
            }
        }
        public GroupEntity Create(string displayName, string name, string userCreated, HashSet<string> memembers, HashSet<string> masterGroup)
        {
            var item = new GroupEntity()
            {
                Created = DateTime.Now,
                CreateUser = userCreated,
                DisplayName = displayName,
                MasterGroup = masterGroup,
                Members = memembers,
                Name = name,
                Status = true,
                IsPrivateChat = false
            };
            Collection.InsertOne(item);
            return item;
        }
        public GroupEntity ChangeDisplayName(string id, string displayName)
        {
            var item = GetItemByID(id);
            if (item != null)
            {
                item.DisplayName = displayName;
                CreateOrUpdate(item);
            }
            return item;
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
