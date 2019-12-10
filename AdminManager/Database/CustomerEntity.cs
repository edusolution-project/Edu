using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminManager.Database
{
    public class CustomerEntity : EntityBase
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Domain { get; set; }
        public string Accounts { get; set; }
        public string Address { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
