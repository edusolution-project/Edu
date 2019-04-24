using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityBaseNet;
using GlobalNet.Utils;
using GlobalNet;
using EntityBaseNet.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet;
using ServiceBaseNet.Services;
using Controller = ServiceBaseNet.ControllerBase;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;

namespace AdminPage.Controllers
{
    [CusMenu(CModule = "Roles",Icon = "verified_user",Color ="danger", Name = "Nhóm người dùng",Type = "Sys")]
    public class RolesController : Controller
    {
        private readonly SysRoleServices _sysRoles;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SysAccessServices _sysAccess;
        private readonly string _keyCache;
        private const string _keyMessageSuccess = "success";
        private const string _keyMessageWarning = "warning";
        private const string _keyMessageError = "error";
        public RolesController(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _sysRoles = new SysRoleServices();
            _sysAccess = new SysAccessServices();
            if(AuthenticationExtends.CurrentUser != null)
            {
                _keyCache = AuthenticationExtends.CurrentUser.Email;
            }
        }
        // GET: Roles
        public ActionResult Index(RolesModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year,model.StartDate.Month,model.StartDate.Day,0,0,0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;
            
            var data = _sysRoles.DbQuery.Where(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o=>o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();

            ViewBag.Data = data.Skip(model.PageSize * model.PageIndex).Take(model.PageSize).ToList();
            model.TotalRecord = data.Count;
            ViewBag.Model = model;
            var message = CacheExtends.GetDataFromCache<Message>(_keyCache + "delete");
            if(message != null)
            {
                ViewBag.Message = message.Content +" ("+ message.Number + " đối tượng)";
                ViewBag.TypeMsg = message.Type;
                CacheExtends.ClearCache(_keyCache + "delete");
            }
            return View();
        }
        public IActionResult Create(RolesModel model)
        {
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RolesModel model,SysRole role)
        {
            ViewBag.Title = "Thêm mới";
            if (model.ID > 0 || role.ID > 0)
            {
                return RedirectToAction("Edit", new { model.ID });
            }
            else
            {
                role.Code = UnicodeName.ConvertUnicodeToCode(role.Name, "-",true);
                await _sysRoles.InsertItemAsync(role);
            }
            return View();
        }
        public IActionResult Edit(int ID)
        {
            RolesModel model = new RolesModel
            {
                ID = ID
            };
            ViewBag.Title = "Chỉnh sửa";
            if (ID == 0)
            {
                return RedirectToAction("Create");
            }
            else
            {
                var item = _sysRoles.GetItemByID(ID);
                if(item == null)
                {
                    ViewBag.Message = "Not Found Data";
                }
                ViewBag.Data = item;
            }
            
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RolesModel model, SysRole role)
        {
            ViewBag.Title = "Chỉnh sửa";
            if (model.ID <= 0 && role.ID <= 0)
            {
                ViewBag.Message = "Chưa chọn đối tượng đê sửa";
            }
            else
            {
                int ID = model.ID > 0 ? model.ID : role.ID;
                var item = _sysRoles.GetItemByID(ID);
                role.ID = item.ID;
                role.Code = UnicodeName.ConvertUnicodeToCode(role.Name, "-", true);
                await _sysRoles.InsertItemAsync(role);

                ViewBag.Data = _sysRoles.GetItemByID(ID);
            }
            ViewBag.Model = model;
            return RedirectToAction("index");
        }
        public IActionResult Permission(RolesModel model)
        {
            if (model.ID > 0)
            {
                ViewBag.Control = MenuExtends.GetMenu();
                ViewBag.Access = _sysAccess.DbQuery.Where(o => o.RoleID == model.ID).ToList();
            }
            else
            {
                ViewBag.Message = "Chưa chọn đối tượng phân quyền";
            }
            ViewBag.Model = model;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Permission(RolesModel model, SysRole role)
        {
            var arr = model.ArrID.Split(',').ToList();
            int count = arr.Count;
            List<SysAccess> data = new List<SysAccess>();
            for(int i = 0; i < count ; i++)
            {
                var iview = arr[i].Split('|');

                string cname = iview[0];
                string aname = iview[1];
                bool isTrue = iview[2] == "true";
                SysAccess item = new SysAccess()
                {
                    ControlName = cname,
                    ActionName = aname,
                    Activity = isTrue,
                    RoleID = model.ID
                };
                await _sysAccess.InsertItemAsync(item);
            }

            CacheExtends.ClearCache(AuthenticationExtends.CurrentUser.RoleID.ToString() + CacheExtends.CacheAccessDefault);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Delete(RolesModel model)
        {
            Message messageSuccess = new Message();
            Message messageError = new Message();
            Message messageWarning = new Message();
            string msg = "";
            string type = "success";
            int delete = 0;
            if (string.IsNullOrEmpty(model.ArrID))
            {
                msg = "Dữ liệu trống";
                type = "danger";
            }
            else
            {
                var arr = model.ArrID.Split(',').Select(int.Parse).ToList();
                int arrCount = arr != null ? arr.Count : 0;
                for(int i = 0; i < arrCount; i++)
                {
                    int id = arr[i];
                    var item = _sysRoles.GetItemByID(id);
                    if(item != null && item.Lock == false)
                    {
                        var Accs = _sysAccess.GetPermisstion(item.ID);
                        for(int x = 0; Accs != null && x < Accs.Count; x++)
                        {
                            await _sysAccess.RemoveItemAsync(Accs[x]);
                        }
                        await _sysRoles.RemoveItemAsync(item);
                        delete++;
                        msg = "đã xóa";
                    }
                    else
                    {
                        msg += "đối tượng không thuộc quyền hạn của bạn";
                        type = "warning";
                    }
                    
                }
            }
            Message message = new Message()
            {
                Content = msg,
                Number = delete,
                Type = type
            };
            CacheExtends.SetObjectFromCache(_keyCache + "delete", 240, message);
            return RedirectToAction("Index","Roles");
        }
        [HttpPost]
        public void Export(RolesModel model)
        {
            DateTime startDate = model.StartDate > DateTime.MinValue ? new DateTime(model.StartDate.Year, model.StartDate.Month, model.StartDate.Day, 0, 0, 0) : DateTime.MinValue;
            DateTime endDate = model.EndDate > DateTime.MinValue ? new DateTime(model.EndDate.Year, model.EndDate.Month, model.EndDate.Day, 23, 59, 59) : DateTime.MinValue;

            var data = _sysRoles.DbQuery.Where(!string.IsNullOrEmpty(model.SearchText), o => o.Name.Contains(model.SearchText) || o.Code.Contains(model.SearchText))
                .Where(model.ID > 0, o => o.ID == model.ID)
                .Where(startDate > DateTime.MinValue, o => o.Created >= startDate)
                .Where(endDate > DateTime.MinValue, o => o.Created <= endDate)
                .OrderByDescending(o => o.ID)
                .ToList();
            var grid = new GridView
            {
                DataSource = data
            };
            grid.DataBind();

            Response.Clear();
            Response.Headers["content-disposition"] ="attachment;filename=Catalog.xls";
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(sw);

            grid.RenderControl(htmlTextWriter);
            Response.WriteAsync(sw.ToString());
            return;
        }
        
    }
}