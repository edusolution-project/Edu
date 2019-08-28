using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
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
        public string GroupID { get; set; }
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
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
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
    }
    public class CalendarService : ServiceBase<CalendarEntity>
    {
        public CalendarService(IConfiguration configuration) : base(configuration)
        {

        }

    }
}
