using BaseCustomerEntity.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class CourseHelper : ICourseHelper
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _service;
        private readonly CourseService _courseService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;

        public CourseHelper(
            GradeService gradeservice
            , SubjectService subjectService
            , TeacherService teacherService
            , ClassService service
            , CourseService courseService
            , ChapterService chapterService
            , LessonService lessonService
            , LessonScheduleService lessonScheduleService
            , StudentService studentService
        )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            _teacherService = teacherService;
            _courseService = courseService;
            _service = service;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
        }

        public List<CourseEntity> GetListCourseByStudent(string studentid)
        {
            throw new NotImplementedException();
        }

        public List<CourseEntity> GetListCourseByTeacher(string teacherid)
        {
            throw new NotImplementedException();
        }
    }
}
