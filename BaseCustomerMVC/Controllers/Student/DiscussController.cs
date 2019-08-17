using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseHub.Database;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class DiscussController : StudentController
    {
        private readonly ClassService _classService;
        private readonly GroupService _groupService;
        private readonly NewFeedService _newFeedService;
        private readonly CommentService _commentService;
        private readonly ChatService _chatService;
        private readonly ChatPrivateService _chatPrivateService;
        private readonly FileProcess _fileProcess;
        public DiscussController(ClassService classService, NewFeedService newFeedService, 
            CommentService commentService, 
            GroupService groupService,
            FileProcess fileProcess)
        {
            _classService = classService;
            _newFeedService = newFeedService;
            _commentService = commentService;
            _groupService = groupService;
            _fileProcess = fileProcess;
        }
        public IActionResult Index()
        {
            ViewBag.Data = _classService.Collection.Find(o => o.Students.Contains(User.Claims.GetClaimByType("UserID").Value)).ToList();
            return View();
        }

        public IActionResult Detail(string id)
        {

            return View();
        }

        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetListNewFeed(string classID, int skip, int take = 0)
        {
            var group = _groupService.GetItemByName(classID);
            if (group == null) return Task.FromResult(new JsonResult(null));
            var listData = _newFeedService.Collection.Find(o => o.GroupID == group.ID);
            if (listData.Count() == 0) return Task.FromResult(new JsonResult(null));
            if (listData.Count() >= take)
            {
                var data = listData.SortBy(o => o.TimePost).Skip(take).Limit(skip).ToList();
                return Task.FromResult(new JsonResult(data));
            }
            else
            {
                return Task.FromResult(new JsonResult(listData));
            }
        }
        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetListComment(string newFeedID,int skip,int take)
        {
            var newFeed = _newFeedService.GetItemByID(newFeedID);
            if (newFeed == null) return Task.FromResult(new JsonResult(null));
            var listData = _commentService.Collection.Find(o => o.NewFeedID == newFeed.ID);
            if (listData.Count() == 0) return Task.FromResult(new JsonResult(null));
            if (listData.Count() >= skip)
            {
                var data = listData.SortBy(o => o.TimePost).Skip(take).Limit(skip).ToList();
                return Task.FromResult(new JsonResult(data));
            }
            else
            {
                return Task.FromResult(new JsonResult(listData));
            }
        }
        [HttpPost]
        public Task<JsonResult> PostNewFeed()
        {
            var files = HttpContext.Request.Form.Files;

            return null;
        }
    }
}
