using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ClassScheduleViewModel : CourseEntity
    {

        public ClassScheduleViewModel(CourseEntity item)
        {
            ID = item.ID;
            Name = item.Name;
            GradeID = item.GradeID;
            SubjectID = item.SubjectID;
            Created = item.Created;
            Updated = item.Updated;
            IsActive = item.IsActive;
            IsAdmin = item.IsAdmin;
            Order = item.Order;
        }

        [JsonProperty("Chapters")]
        public List<ChapterEntity> Chapters { get; set; }
        [JsonProperty("Lessons")]
        public List<LessonScheduleViewModel> Lessons { get; set; }
    }
}
