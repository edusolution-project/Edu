using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ExamEntity : EntityBase
    {
        [JsonProperty("LessonScheduleID")]
        public string LessonScheduleID { get; set; }
        [JsonProperty("StudentID")]
        public string StudentID { get; set; } // admin/student/teacher
        [JsonProperty("Status")]
        public bool Status { get; set; }
        [JsonProperty("Number")]
        public int Number { get; set; }
        [JsonProperty("CurrentDoTime")]
        public DateTime CurrentDoTime { get; set; }
        [JsonProperty("Point")]
        public double Point { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }

    }
    public class ExamService : ServiceBase<ExamEntity>
    {
        public ExamService(IConfiguration configuration) : base(configuration)
        {

        }
        /// <summary>
        /// ID Exam
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public bool IsOverTime(string ID)
        {
            var item = GetItemByID(ID);
            if (item == null) return false;
            return (DateTime.Now - item.CurrentDoTime).TotalSeconds <= 0;
        }
        
    }
}
