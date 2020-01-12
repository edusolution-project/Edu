using Core_v2.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminManager.Database
{
    public class CusAccount : EntityBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string TemPassword { get; set; }
    }
}
