using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
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
