using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using BaseCustomerEntity.Database;
using BaseHub;
using BaseHub.Database;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class DiscussController : TeacherController
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
            return View();
        }

        [HttpPost]
        [Obsolete]
        public Task<JsonResult> GetListNewFeed(string classID, int skip, int take = 0, int FeedType = -1)
        {
            if (string.IsNullOrEmpty(classID)) return Task.FromResult(new JsonResult(null));
            var group = _groupService.GetItemByName(classID);
            if (group == null)
            {
                _groupService.AddMember(classID, User?.FindFirst("UserID").Value);
            }
            var filter = new List<FilterDefinition<NewFeedEntity>> {
                Builders<NewFeedEntity>.Filter.Eq(o => o.GroupID, classID),
                Builders<NewFeedEntity>.Filter.Eq(o=>o.ParentID, null)
            };
            if (FeedType > 0)
                filter.Add(Builders<NewFeedEntity>.Filter.Eq(o => o.FeedType, FeedType));

            var listData = _newFeedService.Collection.Find(Builders<NewFeedEntity>.Filter.And(filter));
            if (listData.Count() == 0) return Task.FromResult(new JsonResult(null));
            if (listData.Count() >= take)
            {
                var data = listData.SortByDescending(o => o.TimePost).Skip(skip).Limit(take).ToList();
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
                var data = listData.SortByDescending(o => o.TimePost).Skip(skip).Limit(take).ToList();
                return new JsonResult(data);
            }
            else
            {
                return new JsonResult(listData.SortByDescending(o => o.TimePost).ToList());
            }
        }

        [HttpPost]
        [Obsolete]
        public JsonResult ToggleFeedType(NewFeedEntity entity)
        {
            var newFeed = _newFeedService.GetItemByID(entity.ID);
            if (newFeed == null)
                return new JsonResult(new Dictionary<string, object> {
                        {"Error", "Thông tin không chính xác" }
                    });
            newFeed.FeedType = entity.FeedType;
            _newFeedService.CreateOrUpdate(newFeed);
            return new JsonResult(new Dictionary<string, object> {
                        {"Data", newFeed }
                    });
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
            if (!string.IsNullOrEmpty(item.ParentID))
                _newFeedService.CreateQuery().UpdateMany(o => o.ID == item.ParentID, Builders<NewFeedEntity>.Update.Inc(t => t.ReplyCount, 1));

            _hubContext.Clients.Group(item.GroupID).SendAsync("CommentNewFeed", Newtonsoft.Json.JsonConvert.SerializeObject(item));

            return Task.FromResult<object>(new { StatusCode = 200 });
        }


    }
}
