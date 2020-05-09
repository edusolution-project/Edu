using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Globals
{
    public class CalendarViewModel : CalendarEntity
    {
        [JsonProperty("LinkLesson")]
        public string LinkLesson { get; set; }
    }
}
