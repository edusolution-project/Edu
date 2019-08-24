using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseHub;
using BaseHub.Database;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        private readonly IHubContext<MyHub> _hubContext;
        public DiscussController(ClassService classService, NewFeedService newFeedService,
            CommentService commentService,
            GroupService groupService,
            FileProcess fileProcess,
            IHubContext<MyHub> hubContext)
        {
            _classService = classService;
            _newFeedService = newFeedService;
            _commentService = commentService;
            _groupService = groupService;
            _fileProcess = fileProcess;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            ViewBag.Data = _classService.Collection.Find(o => o.Students.Contains(User.Claims.GetClaimByType("UserID").Value)).ToList();
            return View();
        }

        public IActionResult Detail(string id)
        {
            ViewBag.ClassID = id;
            return View();
        }

        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetListNewFeed(string classID, int skip, int take = 0)
        {
            if (string.IsNullOrEmpty(classID)) return Task.FromResult(new JsonResult(null));
            var group = _groupService.GetItemByName(classID);
            if (group == null)
            {
                _groupService.AddMemeber(classID, User?.FindFirst("UserID").Value);
            }
            var listData = _newFeedService.Collection.Find(o => o.GroupID == classID && !string.IsNullOrEmpty(o.Content) && string.IsNullOrEmpty(o.ParentID));
            if (listData.Count() == 0) return Task.FromResult(new JsonResult(null));
            if (listData.Count() >= take)
            {
                var data = listData.SortByDescending(o => o.TimePost).Skip(take).Limit(skip).ToList();
                return Task.FromResult(new JsonResult(data));
            }
            else
            {
                return Task.FromResult(new JsonResult(listData.SortByDescending(o => o.TimePost).ToList()));
            }
        }
        [HttpPost]
        [Obsolete]
        public JsonResult GetListComment(string newFeedID, int skip, int take)
        {
            var newFeed = _newFeedService.GetItemByID(newFeedID);
            if (newFeed == null) return new JsonResult(null);
            var listData = _newFeedService.Collection.Find(o => o.ParentID == newFeed.ID);
            if (listData.Count() == 0) return new JsonResult(null);
            if (listData.Count() >= take)
            {
                var data = listData.SortByDescending(o => o.TimePost).Skip(take).Limit(skip).ToList();
                return new JsonResult(data);
            }
            else
            {
                return new JsonResult(listData.SortByDescending(o => o.TimePost).ToList());
            }
        }
        [HttpPost]
        public Task<object> PostNewFeed(NewFeedEntity item)
        {
            //var files = HttpContext.Request.Form.Files;
            //string _html_encode = content;//HttpUtility.HtmlEncode(msg);
            item.Poster = User.Claims.GetClaimByType("UserID").Value;
            item.TimePost = DateTime.Now;
            item.PosterName = User.Identity.Name;

            _newFeedService.CreateOrUpdate(item);
            _hubContext.Clients.Group(item.GroupID).SendAsync("CommentNewFeed", Newtonsoft.Json.JsonConvert.SerializeObject(item));

            return Task.FromResult<object>(new { StatusCode = 200 });
        }
        //[HttpPost]
        //public Task<object> PostComment(string title, string content, string groupID,string parentID)
        //{
        //    //var files = HttpContext.Request.Form.Files;
        //    string _html_encode = content;//HttpUtility.HtmlEncode(msg);
        //    var item = new NewFeedEntity()
        //    {
        //        ParentID = parentID,
        //        Title = title,
        //        Content = _html_encode,
        //        GroupID = groupID,
        //        Poster = User.Claims.GetClaimByType("UserID").Value,
        //        TimePost = DateTime.Now,
        //        PosterName = User.Identity.Name
        //    };
        //    _newFeedService.CreateOrUpdate(item);
        //    _hubContext.Clients.Group(groupID).SendAsync("CommentNewFeed", item);

        //    return Task.FromResult<object>(new { StatusCode = 200, Message = "OK" });
        //}
    }
}
