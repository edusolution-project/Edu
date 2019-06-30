using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class LessonScheduleViewModel : LessonEntity
    {

        public LessonScheduleViewModel(LessonEntity item)
        {
            ID = item.ID;
            CourseID = item.CourseID;
            ChapterID = item.ChapterID;
            IsParentCourse = item.IsParentCourse;
            TemplateType = item.TemplateType;
            Point = item.Point;
            Title = item.Title;
            Code = item.Code;
            IsActive = item.IsActive;
            IsAdmin = item.IsAdmin;
            Order = item.Order;
        }

        [JsonProperty("OpeningDate")]
        public DateTime OpeningDate { get; set; }
        [JsonProperty("CloseDate")]
        public DateTime CloseDate { get; set; }
    }
}
