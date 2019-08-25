using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CalendarLogEntity : EntityBase
    {
        public CalendarEntity Calendar { get; set; }
        public int Status { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
    }
    public class CalendarLogService : ServiceBase<CalendarLogEntity>
    {
        public CalendarLogService(IConfiguration configuration) : base(configuration)
        {

        }

    }
}
