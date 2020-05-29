using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    class NewsViewModel:NewsEntity
    {
        [JsonProperty("CategoryName")]
        public List<NewsCategoryEntity> CategoryName { get; set; }
    }
}
