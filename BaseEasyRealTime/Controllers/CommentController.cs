using BaseEasyRealTime.Entities;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace BaseEasyRealTime.Controllers
{
    public class CommentController : Controller
    {
        private readonly IRoxyFilemanHandler _roxy;
        private readonly CommentService _service;
        public CommentController(IRoxyFilemanHandler roxy, CommentService service)
        {
            _roxy = roxy;
            _service = service;
        }
        [HttpPost]
        public JsonResult Create(string parentID, string content)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var files = _roxy.UploadNewFeed(User.Identity.Name, HttpContext);
                    List<FileManagerCore.Globals.MediaResponseModel> media = new List<FileManagerCore.Globals.MediaResponseModel>();
                    files?.TryGetValue("success", out media);
                    var item = new CommentEntity()
                    {
                        ParentID = parentID,
                        Content = content,
                        Medias = media,
                        Created = DateTime.Now,
                        Sender = User.FindFirst(ClaimTypes.Email)?.Value
                    };

                    return new JsonResult(new { code = 200 , msg = "Đăng bài thành công" , data = item });
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Đăng nhập đển viết bài !!! "});
                }
            }
            catch(Exception ex)
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
                        return new JsonResult(new { code = 404, msg = "Không tìm thấy bài đăng"});
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
        [HttpGet]
        [Obsolete]
        public JsonResult Get(string parentID,bool IsReply, long pageIndex, long pageSize)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (string.IsNullOrEmpty(parentID))
                    {
                        var listItem = _service.CreateQuery().Find(_=>_.ParentID == parentID && _.IsReply == IsReply)?.Skip((int)(pageSize*pageIndex))?.Limit((int)pageSize)?.ToList();
                        return new JsonResult(new { code = listItem == null ? 404 : 200, msg = listItem == null ? "Không tìm thấy bài đăng" : "Đã tìm thấy bài viết", data = listItem });
                    }
                    else
                    {
                        return new JsonResult(new { code = 404, msg = "Chưa có comment nào"});
                    }
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Bạn không có quyền xem bài viết này !!! " });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message, data = ex });
            }
        }
    }
}
