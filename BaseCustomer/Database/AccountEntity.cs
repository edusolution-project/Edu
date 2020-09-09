﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class AccountEntity : EntityBase
    {
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("Type")]
        public string Type { get; set; } // admin/student/teacher
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("PassWord")]
        public string PassWord { get; set; }
        [JsonProperty("PassTemp")]
        public string PassTemp { get; set; }
        [JsonProperty("RoleID")]
        public string RoleID { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("UserCreate")]
        public string UserCreate { get; set; }
        [JsonProperty("CreateDate")]
        public DateTime CreateDate { get; set; }
        [JsonProperty("Phone")]
        public string Phone { get; set; }
        [JsonProperty("VerificationCodes")]
        public string VerificationCodes { get; set; }
        [JsonProperty("TimeOut")]
        public DateTime TimeOut { get; set; }

    }
    public class AccountService : ServiceBase<AccountEntity>
    {
        public AccountService(IConfiguration configuration) : base(configuration)
        {

            var indexs = new List<CreateIndexModel<AccountEntity>>
            {
                //UserName_1
                new CreateIndexModel<AccountEntity>(
                    new IndexKeysDefinitionBuilder<AccountEntity>()
                    .Ascending(t => t.UserName)),
                //UserID_1
                new CreateIndexModel<AccountEntity>(
                    new IndexKeysDefinitionBuilder<AccountEntity>()
                    .Ascending(t => t.UserID)),
                //RoleID_1_UserName_1
                new CreateIndexModel<AccountEntity>(
                    new IndexKeysDefinitionBuilder<AccountEntity>()
                    .Ascending(t => t.RoleID).Ascending(t=> t.UserName)),
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }


        public AccountEntity GetAccount(string type, string userName, string passWord)
        {
            var pass = passWord;
            return Collection.Find(o => o.Type == type && o.UserName == userName && (o.PassWord == pass || o.PassTemp == pass))?.FirstOrDefault();
        }

        public AccountEntity GetAccount(string userName, string passWord)
        {
            var pass = passWord;
            return Collection.Find(o => o.UserName == userName && (o.PassWord == pass || o.PassTemp == pass))?.FirstOrDefault();
        }

        public bool IsAvailable(string userName)
        {
            return Collection.CountDocuments(o => o.UserName == userName) > 0;
        }

        public AccountEntity GetAccountByEmail(string email)
        {
            return Collection.Find(o => o.UserName == email)?.SingleOrDefault();
        }
    }

    public class ACCOUNT_TYPE
    {
        public const string ADMIN = "admin",
            TEACHER = "teacher",
            STUDENT = "student";
    }


    public class PERMISSION
    {
        public const int
            ACCOUNT_EDIT = 100,
            SUBJECT_EDIT = 110,
            GRADE_EDIT = 120,
            COURSE_EDIT = 130,
            MEMBER_COURSE_EDIT = 140,
            CURRICULUM_EDIT = 150,
            CURRICULUM_IMPORT = 151,
            LESSON_EDIT = 160;
    }
}
