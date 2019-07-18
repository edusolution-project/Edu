using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Controllers.Student
{
    public class LearnHistoryController: StudentController
    {
        private readonly LearningHistoryService _service;
        public LearnHistoryController(LearningHistoryService service)
        {
            _service = service;
        }
        [HttpPost]
        public JsonResult GetList()
        {
            string userID = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userID)) return new JsonResult(null);
            var data = _service.CreateQuery().Find(o => o.StudentID == userID)?.ToList();

            return new JsonResult(data);
        }
        [HttpPost]
        public JsonResult GetAll()
        {
            string userID = User.Claims.GetClaimByType("UserID").Value;
            if (string.IsNullOrEmpty(userID)) return new JsonResult(null);
            var data = _service.CreateQuery().Find(_=>true)?.ToList();

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
            if(oldItem!= null)
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
