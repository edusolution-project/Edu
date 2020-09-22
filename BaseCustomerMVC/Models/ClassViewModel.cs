﻿using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassViewModel : ClassEntity
    {
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("SkillName")]
        public string SkillName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }
        [JsonProperty("TotalStudents")]
        public long TotalStudents { get; set; }

        public ClassViewModel(ClassEntity item)
        {
            ID = item.ID;
            Code = item.Code;
            Name = item.Name;
            //GradeID = item.GradeID;
            //SubjectID = item.SubjectID;
            //CourseID = item.CourseID;
            TeacherID = item.TeacherID;
            Students = item.Students;
            Created = item.Created;
            Updated = item.Updated;
            IsActive = item.IsActive;
            Image = item.Image;
            IsAdmin = item.IsAdmin;
            StartDate = item.StartDate;
            EndDate = item.EndDate;
            Order = item.Order;
            Skills = item.Skills;
            Members = item.Members;
            Description = item.Description;
            Image = item.Image;
        }
    }
}
