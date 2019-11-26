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
    public class InforContactController : Controller
    {
        private readonly GroupService _groupService;
        public InforContactController(GroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost]
        public JsonResult Create(string name,HashSet<string> members)
        {
            try
            {
                if(User != null && User.Identity.IsAuthenticated)
                {
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;
                    members.Add(user);
                    var item = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        CreateUser = user,
                        DisplayName = name,
                        IsPrivateChat = false,
                        MasterGroup = new HashSet<string>() { user },
                        Members = members,
                        Name = new Guid().ToString(),
                        Status = true
                    };
                    _groupService.CreateOrUpdate(item);

                    return new JsonResult(new { code = 201, msg ="success", data = item });
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch(Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }
        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;

                    var listItem = _groupService.Collection.Find(o => o.Members.Contains(user))?
                        .SortByDescending(o => o.Created)?
                        .ThenByDescending(o => o.IsPrivateChat)?
                        .ThenByDescending(o => o.Status)?
                        .ToList();

                    return new JsonResult(new { code = 200, msg = "success", data = listItem });
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Leave(string groupName)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;
                    var item = _groupService.Collection.Find(o => o.Name == groupName)?.SingleOrDefault();
                    if(item == null) return new JsonResult(new { code = 404, msg = $"group không tồn tại"});
                    item.Members.Remove(user);
                    if (item.MasterGroup.Contains(user))
                    {
                        item.MasterGroup.Remove(user);
                    }
                    _groupService.CreateOrUpdate(item);
                    return new JsonResult(new { code = 200, msg = "success", data = item });
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Kich(string groupName, string userKich)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;
                    var item = _groupService.Collection.Find(o => o.Name == groupName)?.SingleOrDefault();
                    if (item == null) return new JsonResult(new { code = 404, msg = $"group không tồn tại" });
                    if ((item.MasterGroup.Contains(user) || item.MasterGroup.Count == 0) && item.IsPrivateChat == false)
                    {
                        item.Members.Remove(userKich);
                    }
                    _groupService.CreateOrUpdate(item); 

                    return new JsonResult(new { code = 200, msg = "success", data = item });
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }
        [HttpPost]
        public JsonResult Delete(string groupName)
        {
            try
            {
                if (User != null && User.Identity.IsAuthenticated)
                {
                    var user = User.FindFirst(ClaimTypes.Email)?.Value;
                    var item = _groupService.Collection.Find(o => o.Name == groupName)?.SingleOrDefault();
                    if (item == null) return new JsonResult(new { code = 404, msg = $"group không tồn tại" });
                    if ((item.MasterGroup.Contains(user) || item.MasterGroup.Count == 0 )&& item.IsPrivateChat == false)
                    {
                        _groupService.Remove(item.ID);
                        return new JsonResult(new { code = 200, msg = "success", data = item });
                    }
                    else
                    {
                        return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                    }

                    
                }
                else
                {
                    return new JsonResult(new { code = 405, msg = "bạn không đủ quyền !!" });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = 500, msg = ex.Message });
            }
        }
    }
}
