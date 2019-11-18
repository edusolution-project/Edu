using BaseEasyRealTime.Entities;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace BaseEasyRealTime.Controllers
{
    public class ChatController : Controller
    {
        private readonly IRoxyFilemanHandler _roxy;
        private readonly MessageService _service;
        private readonly GroupService _groupService;
        public ChatController(IRoxyFilemanHandler roxy, MessageService service, GroupService groupService)
        {
            _roxy = roxy;
            _service = service;
            _groupService = groupService;
        }
        [HttpPost]
        public JsonResult Create(string content, int state, string receiver)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    string sender = User.FindFirst(ClaimTypes.Email)?.Value;
                    var groupName = _groupService.GetGroupName(sender, receiver);
                    var files = _roxy.UploadNewFeed(User.Identity.Name, HttpContext);
                    List<FileManagerCore.Globals.MediaResponseModel> media = new List<FileManagerCore.Globals.MediaResponseModel>();
                    files?.TryGetValue("success", out media);
                    var item = new MessageEntity()
                    {
                        State = state,
                        Content = content,
                        Medias = media,
                        Created = DateTime.Now,
                        Sender = sender,
                        Receivers = new HashSet<string>() { groupName },
                        Views = new HashSet<string>()
                    };

                    return new JsonResult(new { code = 200, msg = "Đăng bài thành công", data = item });
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Đăng nhập đển viết bài !!! " });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
        [HttpDelete]
        public JsonResult Remove(string id)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var item = _service.GetItemByID(id);
                    if (item != null && item.Sender == User.FindFirst(ClaimTypes.Email)?.Value)
                    {
                        _service.Remove(id);
                        return new JsonResult(new { code = 200, msg = "Xóa bài thành công", data = item });
                    }
                    else
                    {
                        return new JsonResult(new { code = 404, msg = "Không tìm thấy comment" });
                    }
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Bạn không có quyền xóa comment !!! " });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="receiver">email / group name</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Obsolete]
        
        public JsonResult Get(string receiver,long pageIndex, long pageSize)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    string user = User.FindFirst(ClaimTypes.Email).Value;
                    if (!string.IsNullOrEmpty(receiver))
                    {
                        var groupName = _groupService.GetGroupName(user, receiver);
                        var listItem = _service.CreateQuery()?.Find(o => o.Receivers.First() == groupName)?
                            .SortByDescending(o => o.Created)?
                            .Skip((int)(pageSize * pageIndex))?
                            .Limit((int)pageSize)?
                            .ToList();
                        return new JsonResult(new { code = listItem == null ? 404 : 200, msg = listItem == null ? "Không tìm thấy bài đăng" : "Đã tìm thấy bài viết", data = listItem });
                    }
                    else
                    {
                        return new JsonResult(new { code =  404 , msg = "Không tìm thấy"});
                    }
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Bạn không có quyền !!! " });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }

        [HttpPost]
        public JsonResult UpdateView(string id)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        return new JsonResult(new { code = 404, msg = "không tồn tại bài viết" });
                    }
                    else
                    {
                        var item = _service.GetItemByID(id);
                        if (item.Views.Contains(User.FindFirst(ClaimTypes.Email).Value))
                        {
                            return new JsonResult(new { code = 301, msg = "Đã xem" });
                        }
                        else
                        {
                            item.Views.Add(User.FindFirst(ClaimTypes.Email).Value);
                            item.Updated = DateTime.Now;
                            _service.CreateOrUpdate(item);
                        }
                        return new JsonResult(new { code = item == null ? 404 : 200, msg = item == null ? "Không tìm thấy bài đăng" : "Đã xem bài viết", data = item });
                    }
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Bạn không có quyền xóa bài !!! " });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
    }
}
