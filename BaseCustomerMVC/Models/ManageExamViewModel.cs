using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Models
{
    public class ManageExamViewModel : ManageExamEntity
    {
        [JsonProperty("UserName")]
        public String UserName { get; set; }
        [JsonProperty("ClassName")]
        public String ClassName { get; set; }

        public ManageExamViewModel(ManageExamEntity o)
        {
            this.ID = o.ID;
            this.Name = o.Name;
            this.Created = o.Created;
            this.CreateUser = o.CreateUser;
            this.Updated = o.Updated;
            this.Center = o.Center;
        }
    }
}
