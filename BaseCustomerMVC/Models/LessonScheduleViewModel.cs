using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonScheduleViewModel : LessonEntity
    {

        //public LessonScheduleViewModel(LessonEntity item)
        //{
        //    ID = item.ID;
        //    CourseID = item.CourseID;
        //    ChapterID = item.ChapterID;
        //    IsParentCourse = item.IsParentCourse;
        //    TemplateType = item.TemplateType;
        //    Point = item.Point;
        //    Title = item.Title;
        //    Code = item.Code;
        //    IsActive = item.IsActive;
        //    IsAdmin = item.IsAdmin;
        //    Order = item.Order;
        //}

        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }
    }
}
