using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class HistoryTransactionViewModel:HistoryTransactionEntity
    {
        [JsonProperty("Title")]
        public string Title { get; set; }
        [JsonProperty("CenterName")]
        public string CenterName { get; set; }
        [JsonProperty("ClassName")]
        public string ClassName { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("ProductName")]
        public string ProductName { get; set; }

    }
}
