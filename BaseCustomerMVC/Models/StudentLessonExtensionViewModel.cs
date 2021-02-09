using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class StudentLessonExtensionViewModel : LessonEntity
    {
        [JsonProperty("Part")]
        public List<PartExtensionViewModel> Part { get; set; } = new List<PartExtensionViewModel>() { };
    }
}
