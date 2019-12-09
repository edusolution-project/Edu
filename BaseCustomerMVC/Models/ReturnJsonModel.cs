using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class ReturnJsonModel
    {
        [JsonProperty("StatusCode")]
        public int StatusCode { get; set; } // mã trả về 1: thành công, 0: không thành công

        [JsonProperty("StatusDesc")]
        public string StatusDesc { get; set; } // Thông báo trả về

        [JsonProperty("Location")]
        public string Location { get; set; }
    }

    public class ReturnStatus
    {
        public const int SUCCESS = 1,
            ERROR = 0;
    }
}
