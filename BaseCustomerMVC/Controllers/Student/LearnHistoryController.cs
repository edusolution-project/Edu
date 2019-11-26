using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Core_v2.Globals;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    [BaseAccess.Attribule.AccessCtrl("Lịch sử học tập")]
    public class LearnHistoryController : StudentController
    {
        private readonly LearningHistoryService _service;
        private readonly ClassService _classService;
        private readonly LessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly SubjectService _subjectService;
        private readonly MappingEntity<LearningHistoryEntity, LearningHistoryViewModel> _mapping;

        public LearnHistoryController(LearningHistoryService service,
            ClassService classService,
            LessonService lessonService,
            LessonPartService lessonPartService,
            SubjectService subjectService)
        {
            _service = service;
            _classService = classService;
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _subjectService = subjectService;
            _mapping = new MappingEntity<LearningHistoryEntity, LearningHistoryViewModel>();
        }


        public IActionResult Index()
        {
            var userid = User.Claims.GetClaimByType("UserID").Value;
            ViewBag.User = userid;

            var subjectids = _classService.CreateQuery().Find(o => o.Students.Contains(userid)).ToList().Select(x => x.SubjectID).ToList();
            var subject = _subjectService.CreateQuery().Find(t => subjectids.Contains(t.ID)).ToList();

            ViewBag.Subject = subject;

            return View();
        }

        [Obsolete]
        [HttpPost]
        public JsonResult GetList(DefaultModel model, string ClassID = "", string UserID = "", string SubjectID = "")
        {
            if (string.IsNullOrEmpty(UserID))
                UserID = User.Claims.GetClaimByType("UserID").Value;

            var subjects = _subjectService.GetAll().ToList();

            var classFilter = new List<FilterDefinition<ClassEntity>>();
            classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.Students.Contains(UserID)));

            if (!string.IsNullOrEmpty(SubjectID))
            {
                classFilter.Add(Builders<ClassEntity>.Filter.Where(o => o.SubjectID == SubjectID));
            }

            var activeClass = _classService.CreateQuery().Find(Builders<ClassEntity>.Filter.And(classFilter)).ToList();
            var activeClassIDs = activeClass.Select(t => t.ID).ToList();

            var data = (from r in _service.CreateQuery()
                        .Find(o => activeClassIDs.Contains(o.ClassID) && o.Time >= model.StartDate && o.Time <= model.EndDate).ToList()
                        let currentClass = activeClass.SingleOrDefault(o => o.ID == r.ClassID)
                        let subject = subjects.SingleOrDefault(s => s.ID == currentClass.SubjectID)
                        let lesson = _lessonService.GetItemByID(r.LessonID)
                        select _mapping.AutoOrtherType(r, new LearningHistoryViewModel()
                        {
                            ClassID = currentClass.ID,
                            SubjectName = subject.Name,
                            ClassName = currentClass.Name,
                            LessonName = lesson.Title
                        }));

            model.TotalRecord = data.Count();
            var returnData = data == null || data.Count() <= 0 || data.Count() < model.PageSize || model.PageSize <= 0
                ? data.ToList()
                : data.Skip((model.PageIndex - 1) * model.PageSize).Take(model.PageSize).ToList();
            var response = new Dictionary<string, object>
            {
                { "Data", returnData },
                { "Model", model }
            };
            return new JsonResult(response);
        }



        [HttpPost]
        public JsonResult GetAll()
        {
            string userID = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userID)) return new JsonResult(null);
            var data = _service.CreateQuery().Find(_ => true)?.ToList();

            return new JsonResult(data);
        }
        [HttpPost]
        public JsonResult Create(LearningHistoryEntity item)
        {
            string userID = User.Claims.GetClaimByType("UserID").Value;
            var oldItem = _service.CreateQuery().Find(o => o.StudentID == userID
            && o.LessonID == item.LessonID
            && o.ClassID == item.ClassID
            && o.LessonPartID == item.LessonPartID
            && o.QuestionID == item.QuestionID).ToList();
            if (oldItem != null)
            {
                item.Time = DateTime.Now;
                item.State = 0;
            }
            else
            {
                item.State = oldItem.Count;
                item.Time = DateTime.Now;
            }
            _service.CreateOrUpdate(item);

            return new JsonResult(item);

        }
    }
}
