using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class TeacherEntity : EntityBase
    {
        [JsonProperty("TeacherId")]
        public string TeacherId { get; set; } // mã giáo viên
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
        [JsonProperty("Skype")]
        public string Skype { get; set; }
        [JsonProperty("ZoomID")]
        public string ZoomID { get; set; }
        [JsonProperty("Centers")]
        public List<CenterMemberEntity> Centers { get; set; }
    }

    public class TeacherService : ServiceBase<TeacherEntity>
    {
        public TeacherService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<TeacherEntity>>
            {
                new CreateIndexModel<TeacherEntity>(
                    new IndexKeysDefinitionBuilder<TeacherEntity>()
                    .Text(t => t.FullName).Text(t=> t.Email)),
               //Centers.CenterID_1
                new CreateIndexModel<TeacherEntity>(
                    new IndexKeysDefinitionBuilder<TeacherEntity>()
                    .Descending("Centers.CenterID")),
                //Centers.RoleID_1
                new CreateIndexModel<TeacherEntity>(
                    new IndexKeysDefinitionBuilder<TeacherEntity>()
                    .Descending("Centers.RoleID"))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public TeacherEntity GetItemByEmail(string email)
        {
            return Collection.Find(t => t.Email == email).SingleOrDefault();
        }
    }
}
