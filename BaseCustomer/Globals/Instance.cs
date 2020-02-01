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
            services.AddSingleton<AccountService>();
            services.AddSingleton<PermissionService>();
            services.AddSingleton<RoleService>();
            services.AddSingleton<ScoreService>();
            services.AddSingleton<StudentService>();
            services.AddSingleton<TeacherService>();
            services.AddSingleton<AccountLogService>();
            services.AddSingleton<ModProgramService>();
            services.AddSingleton<ModCourseService>();
            services.AddSingleton<ModChapterService>();
            services.AddSingleton<ModSubjectService>();
            services.AddSingleton<ModGradeService>();
            services.AddSingleton<ModLessonService>();
            services.AddSingleton<ModLessonPartService>();
            services.AddSingleton<ModLessonExtendService>();
            services.AddSingleton<ModLessonPartAnswerService>();
            services.AddSingleton<ModLessonPartQuestionService>();

            services.AddSingleton<ProgramService>();
            services.AddSingleton<CourseService>();
            services.AddSingleton<ChapterService>();
            services.AddSingleton<ChapterProgressService>();

            services.AddSingleton<ChapterExtendService>();
            services.AddSingleton<SubjectService>();
            services.AddSingleton<SkillService>();
            services.AddSingleton<GradeService>();
            services.AddSingleton<LessonService>();
            services.AddSingleton<LessonPartService>();
            services.AddSingleton<LessonExtendService>();
            services.AddSingleton<LessonPartAnswerService>();
            services.AddSingleton<LessonPartQuestionService>();
            services.AddSingleton<LessonScheduleService>();
            services.AddSingleton<LessonProgressService>();

            services.AddSingleton<CloneLessonPartService>();
            services.AddSingleton<CloneLessonPartAnswerService>();
            services.AddSingleton<CloneLessonPartQuestionService>();

            services.AddSingleton<ClassService>();
            services.AddSingleton<ClassSubjectService>();
            services.AddSingleton<ClassProgressService>();

            services.AddSingleton<ExamService>();
            services.AddSingleton<ExamDetailService>();
            services.AddSingleton<LearningHistoryService>();
            services.AddSingleton<ReferenceService>();

            services.AddSingleton<ScoreStudentService>();

            services.AddSingleton<CalendarService>();
            services.AddSingleton<CalendarReportService>();
            services.AddSingleton<CalendarLogService>();
            services.AddSingleton<AccessesService>();
            return services;
        }
    }
}
