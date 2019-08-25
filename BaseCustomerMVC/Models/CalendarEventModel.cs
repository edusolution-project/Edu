using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class CalendarEventModel
    {
        public string groupid { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string url { get; set; }
    }
}
