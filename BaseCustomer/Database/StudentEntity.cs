using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class StudentEntity : EntityBase
    {
        [JsonProperty("StudentId")]
        public string StudentId { get; set; } // mã sinh viên
        [JsonProperty("Avatar")]
        public string Avatar { get; set; }
        [JsonProperty("FullName")]
        public string FullName { get; set; } //họ và tên
        [JsonProperty("Email")]
        public string Email { get; set; }
        [JsonProperty("Phone")]
        public string Phone { get; set; }
        [JsonProperty("Address")]
        public string Address { get; set; }
        [JsonProperty("Class")]
        public List<string> Class { get; set; } //danh sách lớp tham gia
        [JsonProperty("DateBorn")]
        public DateTime DateBorn { get; set; }// ngày sinh
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class StudentService : ServiceBase<StudentEntity>
    {
        public StudentService(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
