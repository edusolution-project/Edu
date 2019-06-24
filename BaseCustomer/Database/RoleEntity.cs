using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class RoleEntity: EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public string UserCreate { get; set; }
        public DateTime CreateDate { get; set; }
    }
    public class RoleService : ServiceBase<RoleEntity>
    {
        public RoleService(IConfiguration configuration) : base(configuration)
        {

        }
    }
}
