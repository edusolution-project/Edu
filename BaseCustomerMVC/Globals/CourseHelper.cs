using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class CourseHelper : ICourseHelper
    {
        //private readonly GradeService _gradeService;
        //private readonly SubjectService _subjectService;
        //private readonly TeacherService _teacherService;
        //private readonly StudentService _studentService;
        //private readonly ClassService _classService;
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
            CourseService courseService
            //GradeService gradeservice
            //, SubjectService subjectService
            //, TeacherService teacherService
            //, ClassService classService

            , ChapterService chapterService
            , LessonService lessonService
            , LessonHelper lessonHelper
            , LessonScheduleService lessonScheduleService
            , CalendarHelper calendarHelper
            , CourseChapterService courseChapterService
            , CourseLessonService courseLessonService
        )
        {
            //_gradeService = gradeservice;
            //_subjectService = subjectService;
            //_teacherService = teacherService;
            _courseService = courseService;
            _courseChapterService = courseChapterService;
            _courseLessonService = courseLessonService;
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

        internal async Task<long> CloneChapterForClassSubject(ClassSubjectEntity classSubject, CourseChapterEntity originChapter = null)
        {
            var orgID = originChapter == null ? "0" : originChapter.ID;
            var newID = "0";

            long lessoncounter = 0;
            ChapterEntity newchapter = null;
            if (originChapter != null)
            {
                newchapter = _chapterMapping.AutoOrtherType(originChapter, new ChapterEntity());
                newchapter.OriginID = originChapter.ID;
                newchapter.ClassID = classSubject.ClassID;
                newchapter.ClassSubjectID = classSubject.ID;
                newchapter.ID = null;

                _chapterService.Save(newchapter);

                newID = newchapter.ID;
            }

            var lessons = _courseLessonService.CreateQuery().Find(o => o.CourseID == classSubject.CourseID && o.ChapterID == orgID).ToList();
            if (lessons != null && lessons.Count > 0)
            {
                foreach (var courselesson in lessons)
                {
                    LessonEntity lesson = _lessonMapping.AutoOrtherType(courselesson, new LessonEntity());
                    lesson.ChapterID = newID;
                    lesson.OriginID = courselesson.ID;
                    lesson.ClassID = classSubject.ClassID;
                    lesson.ClassSubjectID = classSubject.ID;
                    lesson.ID = null;
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
                lessoncounter = lessons.Count;
            }

            var subchaps = _courseChapterService.GetSubChapters(classSubject.CourseID, orgID).AsEnumerable();
            if (subchaps.Count() > 0)
                foreach (var chap in subchaps)
                {
                    chap.ParentID = newID;
                    lessoncounter += await CloneChapterForClassSubject(classSubject, chap);
                }

            //if (newchapter != null)
            //{
            //    newchapter.TotalLessons = lessoncounter;
            //    newchapter.PracticeCount = _chapterService.CountChapterPractice(newchapter.ID, newchapter.ClassSubjectID);
            //    _chapterService.Save(newchapter);
            //}
            return lessoncounter;
        }

        public async Task IncreaseCourseChapterCounter(string ID, long lesInc, long examInc, long pracInc, List<string> listid = null)//prevent circular ref
        {
            var r = await _courseChapterService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<CourseChapterEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
            if (r.ModifiedCount > 0)
            {
                if (listid == null)
                    listid = new List<string> { ID };
                else
                    listid.Add(ID);
                var current = _courseChapterService.GetItemByID(ID);
                if (current != null)
                {
                    if (!string.IsNullOrEmpty(current.ParentID) && (current.ParentID != "0"))
                    {
                        if (listid.IndexOf(current.ParentID) < 0)//prevent circular
                            _ = IncreaseCourseChapterCounter(current.ParentID, lesInc, examInc, pracInc, listid);
                    }
                    else
                        _ = IncreaseCourseCounter(current.CourseID, lesInc, examInc, pracInc);
                }
            }
        }

        public async Task IncreaseCourseCounter(string ID, long lesInc, long examInc, long pracInc, List<string> listid = null)//prevent circular ref
        {
            var r = await _courseService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<CourseEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
        }

        internal async Task ChangeLessonPracticeState(CourseLessonEntity lesson)
        {
            if (lesson.ChapterID != "0")
                await IncreaseCourseChapterCounter(lesson.ChapterID, 0, 0, lesson.IsPractice ? 1 : -1);
            else
                await IncreaseCourseCounter(lesson.CourseID, 0, 0, lesson.IsPractice ? 1 : -1);
        }
    }
}
