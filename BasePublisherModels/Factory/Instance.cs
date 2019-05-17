using BasePublisherModels.Database;
using CoreMongoDB.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BasePublisherModels.Factory
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

            services.AddScoped<ModProgramService>();
            services.AddScoped<ModCourseService>();
            services.AddScoped<ModSubjectService>();
            services.AddScoped<ModGradeService>();
            services.AddScoped<ModUnitService>();
            services.AddScoped<ModLessonService>();
            services.AddScoped<ModLessonPartService>();
            services.AddScoped<ModLessonExtendService>();
            services.AddScoped<ModLessonPartAnswerService>();
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
        public static ModGradeService CreateInstanceModBook(string tableName)
        {
            return new ModGradeService(_configuration, tableName);
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
        public static ModProgramService CreateInstanceModProgram()
        {
            string nameService = typeof(ModProgramService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModProgramService(_configuration, tableName);
        }
        public static ModUnitService CreateInstanceModUnit()
        {
            string nameService = typeof(ModUnitService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModUnitService(_configuration, tableName);
        }
        public static ModSubjectService CreateInstanceModSubject()
        {
            string nameService = typeof(ModSubjectService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModSubjectService(_configuration, tableName);
        }
        public static ModLessonPartService CreateInstanceModLessonPart()
        {
            string nameService = typeof(ModLessonPartService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModLessonPartService(_configuration, tableName);
        }
        public static ModLessonService CreateInstanceModLesson()
        {
            string nameService = typeof(ModLessonService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModLessonService(_configuration, tableName);
        }
        public static ModGradeService CreateInstanceModGrade()
        {
            string nameService = typeof(ModGradeService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new ModGradeService(_configuration, tableName);
        }
        public static CPAccessService CreateInstanceCPAccess()
        {
            string nameService = typeof(CPAccessService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPAccessService(_configuration, tableName);
        }
        public static CPLangService CreateInstanceCPLang()
        {
            string nameService = typeof(CPLangService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPLangService(_configuration, tableName);
        }
        public static CPLoginLogService CreateInstanceCPLoginLog()
        {
            string nameService = typeof(CPLoginLogService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPLoginLogService(_configuration, tableName);
        }
        public static CPMenuService CreateInstanceCPMenu()
        {
            string nameService = typeof(CPMenuService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPMenuService(_configuration, tableName);
        }
        public static CPResourceService CreateInstanceCPResource()
        {
            string nameService = typeof(CPResourceService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPResourceService(_configuration, tableName);
        }
        public static CPRoleService CreateInstanceCPRole()
        {
            string nameService = typeof(CPRoleService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPRoleService(_configuration, tableName);
        }
        public static CPUserService CreateInstanceCPUser()
        {
            string nameService = typeof(CPUserService).Name;
            string tableName = nameService.Replace("Service", "").EndsWith("s")
                ? nameService.Replace("Service", "") : nameService.Replace("Service", "s");
            return new CPUserService(_configuration, tableName);
        }
        #endregion
    }
}
