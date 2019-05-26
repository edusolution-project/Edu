using BaseMongoDB.Database;
using CoreMongoDB.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

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

            services.AddScoped<ModProgramService>();
            services.AddScoped<ModBookService>();
            services.AddScoped<ModSubjectService>();
            services.AddScoped<ModGradeService>();
            
            services.AddScoped<ModUnitService>();
            services.AddScoped<ModLessonService>();
            services.AddScoped<ModLessonPartService>();
            return services;
        }
        /// <summary>
        ///  Get IConfiguration Để khởi tạo service
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configuration"></param>
        public static IApplicationBuilder GetConfiguration(this IApplicationBuilder app, IConfiguration configuration)
        {
            _configuration = configuration;
            var data = QueryCache.GetDataFromCache<IConfiguration>("Configuration_Instance");
            if (data == null) QueryCache.SetObjectFromCache("Configuration_Instance", 480, _configuration);
            return app;
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
        #region Khởi tạo service 

        public static T CreateInstance<T>(this IServiceProvider serviceProvider)
        {
            var service = (T)serviceProvider.GetService(typeof(T));
            return service;
        }

        public static ModProgramService CreateInstanceModProgram(string tableName)
        {
            return new ModProgramService(_configuration, tableName);
        }
        public static ModUnitService CreateInstanceModUnit(string tableName)
        {
            return new ModUnitService(_configuration, tableName);
        }
        public static ModSubjectService CreateInstanceModSubject(string tableName)
        {
            return new ModSubjectService(_configuration, tableName);
        }
        public static ModLessonPartService CreateInstanceModLessonPart(string tableName)
        {
            return new ModLessonPartService(_configuration, tableName);
        }
        public static ModLessonService CreateInstanceModLesson(string tableName)
        {
            return new ModLessonService(_configuration, tableName);
        }
        public static ModGradeService CreateInstanceModGrade(string tableName)
        {
            return new ModGradeService(_configuration, tableName);
        }
        public static ModBookService CreateInstanceModBook(string tableName)
        {
            return new ModBookService(_configuration, tableName);
        }

        public static CPAccessService CreateInstanceCPAccess(string tableName)
        {
            return new CPAccessService(_configuration, tableName);
        }
        public static CPLangService CreateInstanceCPLang(string tableName)
        {
            return new CPLangService(_configuration, tableName);
        }
        public static CPLoginLogService CreateInstanceCPLoginLog(string tableName)
        {
            return new CPLoginLogService(_configuration, tableName);
        }
        public static CPMenuService CreateInstanceCPMenu(string tableName)
        {
            return new CPMenuService(_configuration, tableName);
        }
        public static CPResourceService CreateInstanceCPResource(string tableName)
        {
            return new CPResourceService(_configuration, tableName);
        }
        public static CPRoleService CreateInstanceCPRole(string tableName)
        {
            return new CPRoleService(_configuration, tableName);
        }
        public static CPUserService CreateInstanceCPUser(string tableName)
        {
            return new CPUserService(_configuration, tableName);
        }
        public static SysPageService CreateInstanceSysPage(string tableName)
        {
            return new SysPageService(_configuration, tableName);
        }
        public static SysTemplateService CreateInstanceSysTemplate(string tableName)
        {
            return new SysTemplateService(_configuration, tableName);
        }
        public static SysTemplateDetailService CreateInstanceSysTemplateDetail(string tableName)
        {
            return new SysTemplateDetailService(_configuration, tableName);
        }
        public static SysTemplatePropertyService CreateInstanceSysTemplateProperty(string tableName)
        {
            return new SysTemplatePropertyService(_configuration, tableName);
        }
        #endregion
    }
}
