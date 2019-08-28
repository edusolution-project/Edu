using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class CalendarReportEntity : EntityBase
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string CreateUser { get; set; }
        public DateTime Created { get; set; }
    }
    public class CalendarReportService : ServiceBase<CalendarReportEntity>
    {
        public CalendarReportService(IConfiguration configuration) : base(configuration)
        {

        }

    }
}
