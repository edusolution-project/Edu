using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Admin
{
    [BaseAccess.Attribule.AccessCtrl("Trang chủ", "admin", false)]
    public class HomeController : AdminController
    {
        private readonly CourseLessonService _lessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _questionService;

        private readonly LessonService _clonelessonService;
        private readonly CloneLessonPartService _clonelessonPartService;
        private readonly CloneLessonPartQuestionService _clonequestionService;
        private readonly List<string> quizType = new List<string> { "QUIZ1", "QUIZ2", "QUIZ3", "QUIZ4", "ESSAY" };



        public HomeController(
                CourseLessonService lessonService,
                LessonPartService lessonPartService,
                LessonPartQuestionService questionService,

                LessonService clonelessonService,
                CloneLessonPartService clonelessonPartService,
                CloneLessonPartQuestionService clonequestionService
            )
        {
            _lessonService = lessonService;
            _lessonPartService = lessonPartService;
            _questionService = questionService;

            _clonelessonService = clonelessonService;
            _clonelessonPartService = clonelessonPartService;
            _clonequestionService = clonequestionService;
        }

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string SearchText)
        {
            TempData["Error"] = RouteData.Values["Error"];
            ViewBag.Model = SearchText;
            return RedirectToAction("index");
        }

        // GET: Home/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Home/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Home/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Home/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        public JsonResult FixData()
        {
            var courselessons = _lessonService.GetAll().ToEnumerable();
            foreach (var cl in courselessons)
                calculateCloneLessonPoint(cl.ID);
            var lessons = _clonelessonService.GetAll().ToEnumerable();
            foreach (var l in lessons)
                calculateCloneLessonPoint(l.ID);
            return Json("OK");
        }

        private double calculateLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _lessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            foreach (var part in parts)
            {
                if (part.Type == "ESSAY")
                {
                    point += part.Point;
                }
                else
                {
                    point += _questionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                }
            }
            _lessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }

        private double calculateCloneLessonPoint(string lessonId)
        {
            var point = 0.0;
            var parts = _clonelessonPartService.GetByLessonID(lessonId).Where(t => quizType.Contains(t.Type));
            foreach (var part in parts)
            {
                if (part.Type == "ESSAY")
                {
                    point += part.Point;
                }
                else
                {
                    point += _clonequestionService.GetByPartID(part.ID).Count();//trắc nghiệm => điểm = số câu hỏi (mỗi câu 1đ)
                }
            }
            _clonelessonService.UpdateLessonPoint(lessonId, point);
            return point;
        }
    }
}
