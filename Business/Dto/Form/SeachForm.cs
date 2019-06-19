using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dto.Form
{
  public  class SeachForm
    {
       
        public string Token { get; set; }
        public string UserName { get; set; }

        public string Role { get; set; }
        public string subjectID { get; set; }
        public string gradleID { get; set; }
        public string programID { get; set; }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public string teacherID { get; set; }
        public string courseID { get; set; }


    }
}
