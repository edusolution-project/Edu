using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EntityBaseNet;
using GlobalNet.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceBaseNet;
using ServiceBaseNet.Services;
using Controller = ServiceBaseNet.ControllerBase;

namespace AdminPage.Controllers
{
    public class CAccountsController : Controller
    {
        private readonly SysAccountServices _sysAccounts;
        private readonly SysRoleServices _sysRoles;
        private readonly IHttpContextAccessor _httpContext;
        public CAccountsController(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
            _sysAccounts = new SysAccountServices();
            _sysRoles = new SysRoleServices();
        }
        // GET: Accounts
        public ActionResult Index()
        {
            var currentUser = AuthenticationExtends.CurrentUser;
            if(currentUser == null)
            {
                ViewBag.Message = "Kiểm tra lại thông tin đăng nhập ! ,\n Có thể có ai đó đã dùng tài khoản của bạn";
            }
            else
            {
                ViewBag.Data = currentUser;
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(SysAccount item)
        {
            var currentUser = AuthenticationExtends.CurrentUser;
            if (currentUser == null)
            {
                ViewBag.Message = "Kiểm tra lại thông tin đăng nhập ! ,\n Có thể có ai đó đã dùng tài khoản của bạn";
            }
            else
            {
                bool checkChange = false;
                if(currentUser.ID == item.ID)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        if(currentUser.Name != item.Name)
                        {
                            currentUser.Name = item.Name;
                            checkChange = true;
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(item.Avatar))
                    {
                        if(item.Avatar != currentUser.Avatar)
                        {
                            currentUser.Avatar = item.Avatar;
                            checkChange = true;
                        }
                    }
                    if (item.BirthDay > DateTime.MinValue)
                    {
                        if(item.BirthDay != currentUser.BirthDay)
                        {
                            currentUser.BirthDay = item.BirthDay;
                            checkChange = true;
                        }
                    }
                    if (!string.IsNullOrEmpty(item.Pass))
                    {
                        if (Security.Encrypt(item.Pass) != currentUser.Pass)
                        {
                            currentUser.Pass = Security.Encrypt(item.Pass);
                            checkChange = true;
                        }
                    }
                    if (checkChange)
                    {
                        await _sysAccounts.InsertItemAsync(currentUser);
                    }
                }
                else
                {
                    ViewBag.Message = "Đây không phải tài khoản của bạn";
                }
            }
            return View();
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(string Name,string Pass, string Email)
        {
            string _sPass = Security.Encrypt(Pass);
            if (string.IsNullOrEmpty(Name))
            {
                var user = _sysAccounts.GetLogin(Email.ToLower(),_sPass);
                if (user != null)
                {
                    user.Role = _sysRoles.GetItemByID(user.RoleID);
                    await _httpContext.HttpContext.SetLoginAsync(user);

                    var url = HttpContext.Request.Query["ReturnUrl"];
                    if (string.IsNullOrEmpty(url))
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return Redirect(url);
                    }

                    
                }
                ViewBag.Message = "Password hoặc mật khẩu không đúng";
            }
            else
            {
                if(_sysAccounts.GetItemByEmail(Email) == null)
                {
                    SysAccount account = new SysAccount()
                    {
                        Name = Name,
                        Pass = Pass,
                        Email = Email,
                        Activity = true,
                        RoleID = 2,
                        Created = DateTime.Now
                    };
                    await _sysAccounts.InsertItemAsync(account);
                    ViewBag.Message = "Đăng ký thành công";
                }
                else
                {
                    ViewBag.Message = "Email đã tồn tại";
                }
            }
            return View();
        }

        public IActionResult SignOut()
        {
            _httpContext.HttpContext.Logout();
            return RedirectToAction("SignIn");
        }
    }
}