using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class CourseHelper : ICourseHelper
    {
        private readonly GradeService _gradeService;
        private readonly SubjectService _subjectService;
        private readonly TeacherService _teacherService;
        private readonly StudentService _studentService;
        private readonly ClassService _classService;
        private readonly CourseService _courseService;
        private readonly CourseChapterService _courseChapterService;
        private readonly CourseLessonService _courseLessonService;
        private readonly ChapterService _chapterService;
        private readonly LessonService _lessonService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly LessonHelper _lessonHelper;
        private readonly CalendarHelper _calendarHelper;

        private readonly MappingEntity<CourseChapterEntity, ChapterEntity> _chapterMapping = new MappingEntity<CourseChapterEntity, ChapterEntity>();
        private readonly MappingEntity<CourseLessonEntity, LessonEntity> _lessonMapping = new MappingEntity<CourseLessonEntity, LessonEntity>();


        public CourseHelper(
            GradeService gradeservice
            , SubjectService subjectService
            //, TeacherService teacherService
            //, ClassService classService
            , CourseService courseService
            , ChapterService chapterService
            , LessonService lessonService
            , LessonHelper lessonHelper
            , LessonScheduleService lessonScheduleService
            , CalendarHelper calendarHelper
        )
        {
            _gradeService = gradeservice;
            _subjectService = subjectService;
            //_teacherService = teacherService;
            _courseService = courseService;
            //_classService = classService;
            _chapterService = chapterService;
            _lessonService = lessonService;
            _lessonScheduleService = lessonScheduleService;
            //_studentService = studentService;
            _lessonHelper = lessonHelper;
            _calendarHelper = calendarHelper;
        }

        internal void CloneForClassSubject(ClassSubjectEntity classSubject)
        {
            _ = CloneChapterForClassSubject(classSubject);
            _courseService.Collection.UpdateOneAsync(t => t.ID == classSubject.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
        }

        internal async Task CloneChapterForClassSubject(ClassSubjectEntity classSubject, CourseChapterEntity originChapter = null)
        {
            var orgID = originChapter == null ? "0" : originChapter.ID;
            var newID = "0";

            if (originChapter != null)
            {
                ChapterEntity newchapter = _chapterMapping.AutoOrtherType(originChapter, new ChapterEntity()
                {
                    OriginID = originChapter.ID,
                    ClassID = classSubject.ClassID,
                    ClassSubjectID = classSubject.ID
                });
                newchapter.ID = "";
                _chapterService.Save(newchapter);
                newID = newchapter.ID;
            }

            var lessons = _courseLessonService.CreateQuery().Find(o => o.CourseID == classSubject.CourseID && o.ChapterID == orgID).ToList();

            if (lessons != null && lessons.Count > 0)
                foreach (var courselesson in lessons)
                {
                    LessonEntity lesson = _lessonMapping.AutoOrtherType(courselesson, new LessonEntity()
                    {
                        ChapterID = newID,
                        ClassID = classSubject.ClassID,
                        ClassSubjectID = classSubject.ID
                    });

                    _lessonService.Save(lesson);

                    var schedule = new LessonScheduleEntity
                    {
                        ClassID = classSubject.ClassID,
                        ClassSubjectID = classSubject.ID,
                        LessonID = lesson.ID,
                        Type = lesson.TemplateType,
                        IsActive = true
                    };
                    _lessonScheduleService.Save(schedule);
                    _calendarHelper.ConvertCalendarFromSchedule(schedule, "");
                    _lessonHelper.CloneLessonForClassSubject(lesson, classSubject);
                }

            var subchaps = _courseChapterService.GetSubChapters(originChapter.CourseID, orgID);
            if (subchaps.Count > 0)
                foreach (var chap in subchaps)
                {
                    chap.ParentID = newID;
                    await CloneChapterForClassSubject(classSubject, chap);
                }
        }
    }
}
