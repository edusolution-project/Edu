using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class CourseController : TeacherController
    {
        private readonly CourseService _service;
        private readonly ProgramService _programService;
        private readonly SubjectService _subjectService;
        private readonly ChapterService _chapterService;
        private readonly GradeService _gradeService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonExtendService _lessonExtendService;


        private readonly ModCourseService _modservice;
        private readonly ModProgramService _modprogramService;
        private readonly ModSubjectService _modsubjectService;
        private readonly ModChapterService _modchapterService;
        private readonly ModGradeService _modgradeService;
        private readonly ModLessonService _modlessonService;
        private readonly ModLessonPartService _modlessonPartService;
        private readonly ModLessonPartAnswerService _modlessonPartAnswerService;
        private readonly ModLessonPartQuestionService _modlessonPartQuestionService;
        private readonly ModLessonExtendService _modlessonExtendService;
        public CourseController(CourseService service,
                 ProgramService programService,
                 SubjectService subjectService,
                 ChapterService chapterService,
                 GradeService gradeService,
                 LessonService lessonService,
                 LessonPartService lessonPartService,
                 LessonPartAnswerService lessonPartAnswerService,
                 LessonPartQuestionService lessonPartQuestionService,
                 LessonExtendService lessonExtendService, 
                 ModCourseService modservice
                , ModProgramService modprogramService
                , ModSubjectService modsubjectService
                , ModChapterService modchapterService
                , ModGradeService modgradeService
                , ModLessonService modlessonService
                , ModLessonPartService modlessonPartService
                , ModLessonPartAnswerService modlessonPartAnswerService
                , ModLessonPartQuestionService modlessonPartQuestionService
                , ModLessonExtendService modlessonExtendService)
        {
            _service = service;
            _programService = programService;
            _subjectService = subjectService;
            _chapterService = chapterService;
            _gradeService = gradeService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonExtendService = lessonExtendService;
            _modservice = modservice;
            _modprogramService = modprogramService;
            _modsubjectService = modsubjectService;
            _modchapterService = modchapterService;
            _modgradeService = modgradeService;
            _modlessonService = modlessonService;
            _modlessonPartService = modlessonPartService;
            _modlessonPartAnswerService = modlessonPartAnswerService;
            _modlessonPartQuestionService = modlessonPartQuestionService;
            _modlessonExtendService = modlessonExtendService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Clone(string CourseID)
        {
            //[JsonProperty("GradeID")]
            //public string GradeID { get; set; }
            //[JsonProperty("SubjectID")]
            //public string SubjectID { get; set; }
            //[JsonProperty("ProgramID")]
            //public string ProgramID { get; set; }

            var item = _modservice.GetItemByID(CourseID); //publisher
            if(item != null)
            {
                var grade = _modgradeService.GetItemByID(item.GradeID);
                var subject = _modsubjectService.GetItemByID(item.SubjectID);
                var programe = _modprogramService.GetItemByID(item.ProgramID);
                var chapter = _modchapterService.CreateQuery().Find(o => o.CourseID == CourseID).ToList();
                var lesson = _modlessonService.CreateQuery().Find(o => o.CourseID == CourseID || chapter.Select(x=>x.ID).Contains(o.ChapterID)).ToList();
                var lessonpart = _modlessonPartService.CreateQuery().Find(o => lesson.Select(x => x.ID).Contains(o.ParentID)).ToList();
                var lessonQuestion = _modlessonPartQuestionService.CreateQuery().Find(o => lessonpart.Select(x => x.ID).Contains(o.ParentID)).ToList();
                var lessonAnswer = _modlessonPartAnswerService.CreateQuery().Find(o => lessonpart.Select(x => x.ID).Contains(o.ParentID)).ToList();


                

            }
            return new JsonResult(null);
        }
    }
}
