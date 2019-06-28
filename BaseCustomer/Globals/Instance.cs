using BaseCustomerEntity.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Globals
{
    public static class Instance
    {
        public static IServiceCollection AddServiceBase(this IServiceCollection services)
        {
            services.AddTransient<AccountService>();
            services.AddTransient<PermissionService>();
            services.AddTransient<RoleService>();
            services.AddTransient<StudentService>();
            services.AddTransient<TeacherService>();
            services.AddTransient<AccountLogService>();
            return services;
        }
    }
}
