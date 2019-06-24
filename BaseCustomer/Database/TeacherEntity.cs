using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class TeacherEntity : EntityBase
    {
        public string TeacherId { get; set; } // mã sinh viên
        public string Avatar { get; set; }
        public string FullName { get; set; } //họ và tên
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<string> Subjects { get; set; } // list chuyên môn
        public string DateBorn { get; set; }// ngày sinh
        public bool IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class TeacherService : ServiceBase<TeacherEntity>
    {
        public TeacherService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
