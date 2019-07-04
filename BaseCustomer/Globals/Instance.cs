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
            services.AddTransient<ModProgramService>();
            services.AddTransient<ModCourseService>();
            services.AddTransient<ModChapterService>();
            services.AddTransient<ModSubjectService>();
            services.AddTransient<ModGradeService>();
            services.AddTransient<ModLessonService>();
            services.AddTransient<ModLessonPartService>();
            services.AddTransient<ModLessonExtendService>();
            services.AddTransient<ModLessonPartAnswerService>();
            services.AddTransient<ModLessonPartQuestionService>();

            services.AddTransient<ProgramService>();
            services.AddTransient<CourseService>();
            services.AddTransient<ChapterService>();
            services.AddTransient<SubjectService>();
            services.AddTransient<GradeService>();
            services.AddTransient<LessonService>();
            services.AddTransient<LessonPartService>();
            services.AddTransient<LessonExtendService>();
            services.AddTransient<LessonPartAnswerService>();
            services.AddTransient<LessonPartQuestionService>();
            services.AddTransient<LessonScheduleService>();

            services.AddTransient<CloneLessonPartService>();
            services.AddTransient<CloneLessonPartAnswerService>();
            services.AddTransient<CloneLessonPartQuestionService>();

            services.AddTransient<ClassService>();
            services.AddTransient<ClassProgressService>();

            return services;
        }
    }
}
