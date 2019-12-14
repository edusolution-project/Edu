using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaketingExtends
{
    public static class Startup
    {
        public static void AddEmailSettings(this IServiceCollection services, IConfiguration Configuration)
        {
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddSingleton<IEmail, Email>();
        }
    }
}
