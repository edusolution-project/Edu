using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    [Serializable]
    public class StudentLessonScheduleViewModel
    {

        public string id { get; set; }
        public string classID { get; set; }
        public string className { get; set; }
        public string classSubjectID { get; set; }
        public string subjectName { get; set; }
        public string title { get; set; }
        public string lessonID { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public SkillEntity skill { get; set; }
        public bool isLearnt { get; set; }
        public int type { get; set; }
        public string onlineUrl { get; set; }
    }
}
