using BaseMongoDB.Database;
using CoreMongoDB.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BaseMongoDB.Factory
{
    public static class Instance
    {
        static DbQueryCache QueryCache = new DbQueryCache();
        private static IConfiguration _configuration { get; set; }
        /// <summary>
        ///  dependency-injection . add service
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddServiceBase(this IServiceCollection services)
        {
            services.AddScoped<CPAccessService>();
            services.AddScoped<CPLangService>();
            services.AddScoped<CPLoginLogService>();
            services.AddScoped<CPMenuService>();
            services.AddScoped<CPResourceService>();
            services.AddScoped<CPRoleService>();
            services.AddScoped<CPUserService>();
            services.AddScoped<SysPageService>();
            services.AddScoped<SysTemplateService>();
            services.AddScoped<SysTemplateDetailService>();
            services.AddScoped<SysTemplatePropertyService>();
            return services;
        }
        /// <summary>
        ///  Get IConfiguration Để khởi tạo service
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public static void GetConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            _configuration = configuration;
            var data = QueryCache.GetDataFromCache<IConfiguration>("Configuration_Instance");
            if (data == null) QueryCache.SetObjectFromCache("Configuration_Instance", 480, _configuration);
        }
        /// <summary>
        /// lấy Configuration khi cần
        /// </summary>
        public static IConfiguration Configuration {
            get
            {
                var data =  QueryCache.GetDataFromCache<IConfiguration>("Configuration_Instance");
                if (data == null) QueryCache.SetObjectFromCache("Configuration_Instance", 480 ,_configuration);
                return data??_configuration;
            }
        }
        #region Khởi tạo service => kiến trúc IoC
        public static CPAccessService CreateInstanceCPAccess()
        {
            return new CPAccessService(_configuration);
        }
        public static CPLangService CreateInstanceCPLang()
        {
            return new CPLangService(_configuration);
        }
        public static CPLoginLogService CreateInstanceCPLoginLog()
        {
            return new CPLoginLogService(_configuration);
        }
        public static CPMenuService CreateInstanceCPMenu()
        {
            return new CPMenuService(_configuration);
        }
        public static CPResourceService CreateInstanceCPResource()
        {
            return new CPResourceService(_configuration);
        }
        public static CPRoleService CreateInstanceCPRole()
        {
            return new CPRoleService(_configuration);
        }
        public static CPUserService CreateInstanceCPUser()
        {
            return new CPUserService(_configuration);
        }
        public static SysPageService CreateInstanceSysPage()
        {
            return new SysPageService(_configuration);
        }
        public static SysTemplateService CreateInstanceSysTemplate()
        {
            return new SysTemplateService(_configuration);
        }
        public static SysTemplateDetailService CreateInstanceSysTemplateDetail()
        {
            return new SysTemplateDetailService(_configuration);
        }
        public static SysTemplatePropertyService CreateInstanceSysTemplateProperty()
        {
            return new SysTemplatePropertyService(_configuration);
        }
        #endregion
    }
}
