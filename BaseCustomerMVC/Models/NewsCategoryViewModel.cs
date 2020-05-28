using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using BaseCustomerEntity.Database;

namespace BaseCustomerMVC.Models
{
    public class NewsCategoryViewModel:NewsCategoryEntity
    {
        [JsonProperty("ParentName")]
        public List<NewsCategoryEntity> ParentName { get; set; }
    }
}
