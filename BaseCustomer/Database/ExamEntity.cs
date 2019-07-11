﻿using Core_v2.Repositories;
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
        [JsonProperty("Timer")]
        public int Timer { get; set; }
        [JsonProperty("ClassID")]
        public string ClassID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("LessonID")]
        public string LessonID { get; set; }
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
            if (item.Status) return false;
            if (item.Timer == 0) return true;
            return (item.CurrentDoTime.AddMinutes(item.Timer)-DateTime.Now).TotalSeconds <= 0;
        }
        
    }
}
