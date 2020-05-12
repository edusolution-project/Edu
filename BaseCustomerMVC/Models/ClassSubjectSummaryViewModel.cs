﻿using BaseCustomerEntity.Database;
using Newtonsoft.Json;

namespace BaseCustomerMVC.Models
{
    public class StudentSummaryViewModel : ClassSubjectProgressEntity
    {
        [JsonProperty("FullName")]
        public string FullName { get; set; }
        [JsonProperty("Rank")]
        public int Rank { get; set; }
        [JsonProperty("SkillName")]
        public string SkillName { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("TotalStudents")]
        public int TotalStudents { get; set; }
    }
}