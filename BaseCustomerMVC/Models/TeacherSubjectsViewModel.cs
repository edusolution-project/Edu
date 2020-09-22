using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class TeacherSubjectsViewModel : TeacherEntity
    {
        public List<SubjectModel> SubjectList { get; set; }
    }


    public class SubjectModel
    {
        public string BookName { get; set; }
        public string SkillName { get; set; }
    }
}
