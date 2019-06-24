using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class StudentEntity:EntityBase
    {
        public string StudentId { get; set; } // mã sinh viên
        public string Avatar { get; set; }
        public string FullName { get; set; } //họ và tên
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public List<string> Class { get; set; } //danh sách lớp tham gia
        public string DateBorn { get; set; }// ngày sinh
        public bool IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class StudentService : ServiceBase<StudentEntity>
    {
        public StudentService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
