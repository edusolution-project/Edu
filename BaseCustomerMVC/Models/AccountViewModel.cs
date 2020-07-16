﻿using BaseCustomerEntity.Database;
using Core_v2.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class AccountViewModel : AccountEntity
    {
        [JsonProperty("RoleName")]
        public string RoleName { get; set; }
    }
}
