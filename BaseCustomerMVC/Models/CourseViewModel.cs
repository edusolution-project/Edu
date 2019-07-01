using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CourseViewModel:CourseEntity
    {
        public CourseViewModel(CourseEntity item)
        {
            ID = item.ID;
            Name = item.Name;
            GradeID = item.GradeID;
            SubjectID = item.SubjectID;
            TeacherID = item.TeacherID;
            Created = item.Created;
            Updated = item.Updated;
            IsActive = item.IsActive;
            IsAdmin = item.IsAdmin;
            Order = item.Order;
        }
        [JsonProperty("CourseName")]
        public string CourseName { get; set; }
        [JsonProperty("SubjectName")]
        public string SubjectName { get; set; }
        [JsonProperty("GradeName")]
        public string GradeName { get; set; }
        [JsonProperty("TeacherName")]
        public string TeacherName { get; set; }

    }
}
