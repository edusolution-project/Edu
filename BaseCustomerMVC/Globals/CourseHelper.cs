using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
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
        private readonly LessonHelper _lessonHelper;

        private readonly MappingEntity<CourseEntity, CourseEntity> _cloneCourseMapping = new MappingEntity<CourseEntity, CourseEntity>();
        private readonly MappingEntity<CourseChapterEntity, CourseChapterEntity> _cloneCourseChapterMapping = new MappingEntity<CourseChapterEntity, CourseChapterEntity>();
        private readonly MappingEntity<CourseChapterEntity, ChapterEntity> _chapterMapping = new MappingEntity<CourseChapterEntity, ChapterEntity>();
        private readonly MappingEntity<CourseLessonEntity, CourseLessonEntity> _cloneCourseLessonMapping = new MappingEntity<CourseLessonEntity, CourseLessonEntity>();
        private readonly MappingEntity<CourseLessonEntity, LessonEntity> _lessonMapping = new MappingEntity<CourseLessonEntity, LessonEntity>();

        public CourseHelper(
            CourseService courseService,
            CourseChapterService courseChapterService,
            CourseLessonService courseLessonService,

            ChapterService chapterService,

            LessonHelper lessonHelper
        )
        {
            _courseService = courseService;
            _courseChapterService = courseChapterService;
            _courseLessonService = courseLessonService;

            _chapterService = chapterService;

            _lessonHelper = lessonHelper;
        }


        public async Task<CourseEntity> CopyCourse(CourseEntity org_course, CourseEntity target_course)
        {
            var new_course = _cloneCourseMapping.Clone(org_course, new CourseEntity());

            new_course.OriginID = org_course.ID;
            if (!string.IsNullOrEmpty(target_course.Name))
                new_course.Name = target_course.Name;
            new_course.TeacherID = target_course.CreateUser;
            new_course.CreateUser = target_course.CreateUser;
            new_course.Center = target_course.Center ?? org_course.Center;
            new_course.Created = DateTime.UtcNow;
            new_course.Updated = DateTime.UtcNow;
            new_course.IsActive = false;
            new_course.IsUsed = false;
            //new_course.IsPublic = false;
            //new_course.PublicWStudent = false;
            new_course.TargetCenters = new List<string>();
            new_course.StudentTargetCenters = new List<string>();


            _courseService.Collection.InsertOne(new_course);

            await CloneCourseChapter(new CourseChapterEntity
            {
                OriginID = "0",
                CourseID = new_course.ID,
                CreateUser = new_course.CreateUser
            }, org_course.ID);

            return new_course;
        }

        private async Task<CourseChapterEntity> CloneCourseChapter(CourseChapterEntity item, string orgCourseID)
        {
            if (item.OriginID != "0")
                _courseChapterService.Collection.InsertOne(item);
            else
            {
                item.ID = "0";
            }

            var lessons = _courseLessonService.GetChapterLesson(orgCourseID, item.OriginID);

            if (lessons != null && lessons.Count() > 0)
            {
                foreach (var o in lessons)
                {
                    var new_lesson = _cloneCourseLessonMapping.Clone(o, new CourseLessonEntity());
                    new_lesson.CourseID = item.CourseID;
                    new_lesson.ChapterID = item.ID;
                    new_lesson.CreateUser = item.CreateUser;
                    new_lesson.Created = DateTime.UtcNow;
                    new_lesson.OriginID = o.ID;
                    await _lessonHelper.CopyCourseLessonFromCourseLesson(o, new_lesson);
                }
            }

            var subChapters = _courseChapterService.GetSubChapters(orgCourseID, item.OriginID);
            foreach (var o in subChapters)
            {
                var new_chapter = _cloneCourseChapterMapping.Clone(o, new CourseChapterEntity());
                new_chapter.CourseID = item.CourseID;
                new_chapter.ParentID = item.ID;
                new_chapter.CreateUser = item.CreateUser;
                new_chapter.Created = DateTime.UtcNow;
                new_chapter.OriginID = o.ID;
                await CloneCourseChapter(new_chapter, orgCourseID);
            }
            return item;
        }

        internal void CloneForClassSubject(ClassSubjectEntity classSubject)
        {
            _ = CloneChapterForClassSubject(classSubject);
            //_courseService.Collection.UpdateOneAsync(t => t.ID == classSubject.CourseID, new UpdateDefinitionBuilder<CourseEntity>().Set(t => t.IsUsed, true));
        }

        internal async Task<long> CloneChapterForClassSubject(ClassSubjectEntity classSubject, CourseChapterEntity originChapter = null)
        {
            var orgID = originChapter == null ? "0" : originChapter.ID;
            var newID = "0";

            long lessoncounter = 0;
            if (originChapter != null)
            {
                ChapterEntity newchapter = _chapterMapping.AutoOrtherType(originChapter, new ChapterEntity());
                newchapter.OriginID = originChapter.ID;
                newchapter.ClassID = classSubject.ClassID;
                newchapter.ClassSubjectID = classSubject.ID;
                newchapter.ID = null;

                if (!string.IsNullOrEmpty(originChapter.ConnectID) || originChapter.Period > 0) // set lo trinh
                {
                    newchapter.StartDate = classSubject.StartDate.AddDays(originChapter.Start);
                    newchapter.EndDate = newchapter.StartDate.AddDays(originChapter.Period);
                }
                _chapterService.Save(newchapter);
                newID = newchapter.ID;
            }


            var lessons = _courseLessonService.GetChapterLesson(classSubject.CourseID, orgID).OrderBy(t => t.ConnectID).ToList();
            if (lessons != null && lessons.Count() > 0)
            {
                foreach (var courselesson in lessons)
                {
                    var lstart = courselesson.Start;

                    await _lessonHelper.CopyLessonFromCourseLesson(courselesson, new LessonEntity
                    {
                        ChapterID = newID,
                        OriginID = courselesson.ID,
                        ClassID = classSubject.ClassID,
                        ClassSubjectID = classSubject.ID,
                    }, classSubject.StartDate);
                }
                lessoncounter = lessons.Count();
            }

            var subchaps = _courseChapterService.GetSubChapters(classSubject.CourseID, orgID);
            if (subchaps.Count() > 0)
                foreach (var chap in subchaps)
                {
                    chap.ParentID = newID;
                    lessoncounter += await CloneChapterForClassSubject(classSubject, chap);
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
