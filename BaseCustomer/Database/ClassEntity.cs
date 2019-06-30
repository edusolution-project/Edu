﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class ClassEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("GradeID")]
        public string GradeID { get; set; }
        [JsonProperty("SubjectID")]
        public string SubjectID { get; set; }
        [JsonProperty("CourseID")]
        public string CourseID { get; set; }
        [JsonProperty("TeacherID")]
        public string TeacherID { get; set; }
        [JsonProperty("Students")]
        public List<string> Students { get; set; } = new List<string>();
        [JsonProperty("Created")]
        public DateTime Created { get; set; }
        [JsonProperty("OpeningDate")]
        public DateTime OpeningDate { get; set; }

        [JsonProperty("CloseDate")]
        public DateTime CloseDate { get; set; }

        [JsonProperty("Updated")]
        public DateTime Updated { get; set; }
        public bool IsAdmin { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
        [JsonProperty("Order")]
        public int Order { get; set; }

        
    }
    public class ClassService : ServiceBase<ClassEntity>
    {
        public ClassService(IConfiguration config) : base(config)
        {

        }
    }
}
