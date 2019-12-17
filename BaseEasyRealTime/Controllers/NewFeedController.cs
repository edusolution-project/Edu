﻿using BaseEasyRealTime.Entities;
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
    public class NewFeedController : Controller
    {
        private readonly IRoxyFilemanHandler _roxy;
        private readonly NewFeedService _service;
        public NewFeedController(IRoxyFilemanHandler roxy, NewFeedService service)
        {
            _roxy = roxy;
            _service = service;
        }
        [HttpPost]
        public JsonResult Create(string title,string content,int state,HashSet<string> receivers)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var files = _roxy.UploadNewFeed(User.Identity.Name, HttpContext);
                    List<FileManagerCore.Globals.MediaResponseModel> media = new List<FileManagerCore.Globals.MediaResponseModel>();
                    files?.TryGetValue("success", out media);
                    var item = new NewFeedEntity()
                    {
                        Title = title,
                        State = state,
                        Content = content,
                        Name = User.Identity.Name,
                        Medias = media,
                        Created = DateTime.Now,
                        Sender = User.FindFirst(ClaimTypes.Email)?.Value,
                        Receivers = receivers,
                        Views = new HashSet<string>()
                    };
                    _service.CreateOrUpdate(item);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"> new feed id </param>
        /// <param name="state"> 0/publish 1/group 2/user </param>
        /// <param name="receivers"> all, groupname, user </param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Obsolete]
        public JsonResult Get(string id, int state, string receivers, long pageIndex, long pageSize)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        var data = _service.CreateQuery();
                        if(state == 0)
                        {
                            if(data.Count(_=>true && _.State > 0 && (_.Receivers.Contains(receivers)||_.Sender == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value) && _.RemoveByAdmin == false) <= 5)
                            {
                                var realData = data.Find(_ => true && _.State > 0 && (_.Receivers.Contains(receivers) || _.Sender == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value) && _.RemoveByAdmin == false)
                                    ?.SortByDescending(_=>_.Created)
                                    ?.Skip(0)
                                    ?.Limit(5)
                                    ?.ToList();
                                return new JsonResult(new { code = 200, msg = "Success", data = realData?.OrderByDescending(o=>o.Created)?.ToList() });
                            }
                        }

                        if (data.Count(_ => true && _.State == state && (_.Receivers.Contains(receivers) || _.Sender == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value) && _.RemoveByAdmin == false) <= pageSize)
                        {
                            var realData = data.Find(_ => true && _.State == state && (_.Receivers.Contains(receivers) || _.Sender == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value) && _.RemoveByAdmin == false)?.Skip((int)(pageSize * pageIndex)).Limit((int)pageSize)?.ToList();
                            return new JsonResult(new { code = 200, msg = "Success", data = realData });
                        }
                        else
                        {
                            return new JsonResult(new { code = 200, msg = "Success", data = data.Find(_ => true && _.State == state && (_.Receivers.Contains(receivers) || _.Sender == User.FindFirst(System.Security.Claims.ClaimTypes.Email).Value) && _.RemoveByAdmin == false)?.ToList() });
                        }
                    }
                    else
                    {
                        var item = _service.GetItemByID(id);
                        return new JsonResult(new { code = item == null ? 404 : 200, msg = item == null ? "Không tìm thấy bài đăng" : "Đã tìm thấy bài viết", data = item });
                    }
                }
                else
                {
                    return new JsonResult(new { code = 201, msg = "Bạn không có quyền xem bài !!! " });
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
