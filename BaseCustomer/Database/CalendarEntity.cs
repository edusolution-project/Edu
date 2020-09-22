using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CalendarEntity : EntityBase
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("GroupID")]
        public string GroupID { get; set; } // classid
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }

        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }

        [JsonProperty("UrlRoom")]
        public string UrlRoom { get; set; } // appaer
        [JsonProperty("LimitNumberUser")]
        public int LimitNumberUser { get; set; } = 3; // 3 hocjs vien va 1 giao vien 
        [JsonProperty("UserBook")]
        public List<string> UserBook { get; set; } = new List<string>();
        [JsonProperty("CreateUser")]
        public string CreateUser { get; set; }
        [JsonProperty("Status")]
        public int Status { get; set; } = 0;
        [JsonProperty("IsDel")]
        public bool IsDel { get; set; } = false;
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("ScheduleID")]
        public string ScheduleID { get; set; }
        // longht add 2020-01-12
        [JsonProperty("Skype")]
        public string Skype { get; set; }
        [JsonProperty("Content")]
        public string Content { get; set; }

    }
    public class CalendarService : ServiceBase<CalendarEntity>
    {
        public CalendarService(IConfiguration configuration) : base(configuration)
        {
            var indexs = new List<CreateIndexModel<CalendarEntity>>
            {
                //LessonID_1_StartDate_1_EndDate_1
                new CreateIndexModel<CalendarEntity>(
                    new IndexKeysDefinitionBuilder<CalendarEntity>()
                    .Ascending(t=> t.GroupID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate)),
                new CreateIndexModel<CalendarEntity>(
                    new IndexKeysDefinitionBuilder<CalendarEntity>()
                    .Ascending(t=> t.GroupID).Ascending(t=> t.TeacherID).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate)),
                //CreateUser_1_StartDate_1_EndDate_1
                new CreateIndexModel<CalendarEntity>(
                    new IndexKeysDefinitionBuilder<CalendarEntity>()
                    .Ascending(t=> t.CreateUser).Ascending(t=> t.StartDate).Ascending(t=> t.EndDate))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

    }
}
