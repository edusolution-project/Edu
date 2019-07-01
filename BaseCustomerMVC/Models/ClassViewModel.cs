using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassViewModel : ClassEntity
    {

        public ClassViewModel(ClassEntity item)
        {
            ID = item.ID;
            Name = item.Name;
            GradeID = item.GradeID;
            SubjectID = item.SubjectID;
            CourseID = item.CourseID;
            TeacherID = item.TeacherID;
            Students = item.Students;
            Created = item.Created;
            Updated = item.Updated;
            IsActive = item.IsActive;
            IsAdmin = item.IsAdmin;
            StartDate = item.StartDate;
            EndDate = item.EndDate;
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
