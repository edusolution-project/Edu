using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Core_v2.Repositories;

namespace BaseCustomerMVC.Models
{
    public class StudentLessonViewModel : LessonEntity
    {
        [JsonProperty("Parts")]
        public List<PartViewModel> Parts { get; set; } = new List<PartViewModel>() { };

    }
}
