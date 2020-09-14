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
            CourseService courseService,
            CourseChapterService courseChapterService,
            CourseLessonService courseLessonService,

            LessonService lessonService,
            ChapterService chapterService,

            LessonHelper lessonHelper
        )
        {
            _courseService = courseService;
            _courseChapterService = courseChapterService;
            _courseLessonService = courseLessonService;

            _lessonService = lessonService;
            _chapterService = chapterService;

            _lessonHelper = lessonHelper;
        }

        internal void CloneForClassSubject(ClassSubjectEntity classSubject)
        {
            _ = CloneChapterForClassSubject(classSubject);
            _courseService.Collection.UpdateOneAsync(t => t.ID == classSubject.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
        }

        internal long CloneChapterForClassSubject(ClassSubjectEntity classSubject, CourseChapterEntity originChapter = null)
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

            var lessons = _courseLessonService.GetChapterLesson(classSubject.CourseID, orgID);
            if (lessons != null && lessons.Count() > 0)
            {
                foreach (var courselesson in lessons)
                {
                    _lessonHelper.CopyLessonFromCourseLesson(courselesson, new LessonEntity
                    {
                        ChapterID = newID,
                        OriginID = courselesson.ID,
                        ClassID = classSubject.ClassID,
                        ClassSubjectID = classSubject.ID
                    });
                }
                lessoncounter = lessons.Count();
            }

            var subchaps = _courseChapterService.GetSubChapters(classSubject.CourseID, orgID);
            if (subchaps.Count() > 0)
                foreach (var chap in subchaps)
                {
                    chap.ParentID = newID;
                    lessoncounter += CloneChapterForClassSubject(classSubject, chap);
                }

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
