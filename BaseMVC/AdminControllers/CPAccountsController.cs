using CoreLogs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using BaseMVC.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using BaseMongoDB.Database;

namespace BaseMVC.AdminControllers
{
    public class CPAccountsController : Controller
    {
        protected CPUserEntity currentUser;
        private readonly CPUserService _userService;
        private readonly CPLoginLogService _loginLogService;
        private readonly CPRoleService _roleService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogs _ilogs;
        public CPAccountsController(IHostingEnvironment environment,
            CPUserService userService, 
            CPLoginLogService loginLogService,
            CPRoleService roleService)
        {
            _hostingEnvironment = environment;
            _userService = userService;
            _loginLogService = loginLogService;
            _roleService = roleService;
            _ilogs = new Logs(_hostingEnvironment.WebRootPath);
        }
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var email = User.Claims.SingleOrDefault(o => o.Type == ClaimTypes.Email).Value;
                if (!string.IsNullOrEmpty(email)) {
                    currentUser = _userService.GetItemByEmail(email);
                    ViewBag.Data = currentUser;
                }
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string name,string pass, DateTime birthDay, string skype,string phone)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    currentUser.Name = name;
                }
                if (!string.IsNullOrEmpty(pass) && currentUser.Pass != Security.Encrypt(pass))
                {
                    currentUser.Pass = Security.Encrypt(pass);
                }
                if (!string.IsNullOrEmpty(skype))
                {
                    currentUser.Skype = skype;
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    currentUser.Phone = phone;
                }
                if (birthDay > DateTime.MinValue)
                {
                    currentUser.BirthDay = birthDay;
                }
                _userService.Add(currentUser);
            }
            return View();
        }

        public ActionResult SignIn(string returnurl)
        {
            ViewBag.Message = TempData["error"];
            ViewBag.ReturnURL = returnurl;
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string username, string password, string returnurl)
        {
            _ilogs.WriteLogsInfo(username + "-" + password + "-" + returnurl);
            if(string.IsNullOrEmpty(username)|| string.IsNullOrEmpty(password))
            {
                ViewBag.MessageError = "Không thể bỏ trống các trường bắt buộc";
                return View();
            }
            else
            {
                var user = _userService.GetItemByEmail(username);
                if(user == null)
                {
                    ViewBag.MessageError = "Email không tồn tại";
                    return View();
                }
                else
                {
                    if(user.Pass == Security.Encrypt(password))
                    {
                        if (user.IsActive)
                        {
                            TempData["success"] = "Xin chào " + user.Name;
                            string _token = Guid.NewGuid().ToString();
                            var role = _roleService.GetByID(user.RoleID);
                            if (role != null)
                            {
                                HttpContext.SetValue(Cookies.DefaultLogin, _token,Cookies.ExpiresLogin,false);
                                //_ilogs.WriteLogsInfo(_token);
                                var claims = new List<Claim>
                                {
                                    new Claim(ClaimTypes.Email, user.Email),
                                    new Claim(ClaimTypes.Name, user.Name),
                                    new Claim(ClaimTypes.Role, role.Code),
                                    new Claim("RoleID", role.ID.ToString()),
                                };
                                var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
                                _ = new AuthenticationProperties
                                {
                                    IsPersistent = true,
                                    ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
                                };
                                ClaimsPrincipal claim = new ClaimsPrincipal();
                                claim.AddIdentity(claimsIdentity);

                                CPLoginLogEntity login = new CPLoginLogEntity()
                                {
                                    IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                                    Email = user.Email,
                                    Token = _token,
                                    IsActive = true,
                                    Created = DateTime.Now
                                };
                                var url = string.IsNullOrEmpty(returnurl) ? null : System.Net.WebUtility.UrlDecode(returnurl).Split('/');
                                _loginLogService.SetLogin(login);
                                return url == null
                                    ? RedirectToAction("Index", "CPHome")
                                    : RedirectToAction(url[1], url[0]);
                            }
                            else
                            {
                                ViewBag.MessageError = "Bạn không có quyền hạn vào quản trị viện";
                                return View();
                            }
                        }
                        else
                        {
                            ViewBag.MessageError = "Tài khoản của bạn đã bị khóa";
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.MessageError = "PassWord không chính xác";
                        return View();
                    }
                }
            }

        }

        [HttpGet]
        public ActionResult SignOut()
        {
            try
            {
                var token = HttpContext.GetValue(Cookies.DefaultLogin, false);
                if (!string.IsNullOrEmpty(token))
                {
                    _loginLogService.RemoveLog(token);
                }
                HttpContext.Remove(Cookies.DefaultLogin);
                return RedirectToAction("SignIn");
            }
            catch(Exception ex)
            {
                ViewBag.Message = ex.ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string Name, string Email , string PassWord , string TempPass)
        {

            return View();
        }
    }
}
