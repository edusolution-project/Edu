﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class TeacherEntity : EntityBase
    {
        [JsonProperty("TeacherId")]
        public string TeacherId { get; set; } // mã sinh viên
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
        [JsonProperty("Subjects")]
        public List<string> Subjects { get; set; } // list chuyên môn
        [JsonProperty("DateBorn")]
        public DateTime DateBorn { get; set; }// ngày sinh
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
    }
    public class TeacherService : ServiceBase<TeacherEntity>
    {
        public TeacherService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}