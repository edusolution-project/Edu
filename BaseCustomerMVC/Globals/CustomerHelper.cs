using BaseCustomerEntity.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class CustomerHelper : ICustomerHelper
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
        public CustomerHelper(
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
        /// <summary>
        ///  lấy danh sách khóa học 
        /// </summary>
        /// <param name="status"> trạng thái </param>
        /// <param name="seachText"> tên giáo trình, tên bài học </param>
        /// <param name="gradeID"> cấp độ </param>
        /// <param name="teacherId"> Mã giáo viên  </param>
        /// <param name="startDate"> ngày bắt đầu </param>
        /// <param name="endDate"> ngày kết thúc </param>
        /// <param name="subject"> môn học </param>
        private void getCourseForStudent(
            bool status,
            string seachText,
            string gradeID,
            string teacherId,
            DateTime startDate,
            DateTime endDate ,
            string subject)
        {

        } 
        /// <summary>
        /// lấy list bài giảng
        /// </summary>
        /// <param name="courseID"></param>
        private void getCourseDetails(string courseID)
        {

        }
        /// <summary>
        /// Get bài giảng
        /// </summary>
        /// <param name="lessonID"></param>
        private void getLessonDetails(string lessonID)
        {

        }
    }
}
