using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Core_v2.Repositories;

namespace BaseCustomerMVC.Models
{
    public class StudentLessonViewModel : LessonEntity
    {
        [JsonProperty("Part")]
        public List<PartViewModel> Part { get; set; } = new List<PartViewModel>() { };

    }
}
