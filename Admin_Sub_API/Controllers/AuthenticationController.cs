using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Text;

using System;
using NLog;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Configuration;

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using BaseMongoDB.Database;

using Business.Dto.Form;
using BaseMVC.Globals;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class AuthenticationController : ControllerBase
    {
        private static readonly Logger ClassLogger = LogManager.GetCurrentClassLogger();

        //public AuthenticationController(IHttpContextAccessor httpContextAccessor, 
        //    SMEEntities context, IConfiguration configuration,
        //    IHostingEnvironment hostingEnvironment) : base(httpContextAccessor, context, configuration, hostingEnvironment)
        //{
        //}
        CPUserSubService _userService;
      AccessTokenService _accessTokenService;
        public AuthenticationController(
        CPUserSubService userService, AccessTokenService accessTokenService
       )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
        }
        [HttpGet]
        public List<string> test()
        {
            string a = "12";
            return null;
        }
        [HttpPost]
        // [SMEActionAuditFilter]
        public LoginResultForm LogIn([FromBody]AuthenticationForm form)
        {
          
            //NguoiDungService service = GetService<NguoiDungService>();


            //if (string.IsNullOrWhiteSpace(form.Username))
            //{
            //    throw new InvalidUsernameOrPasswordException();
            //}
            //else
            //{
            //    form.Username = form.Username.ToLower();
            //}

            // _ilogs.WriteLogsInfo(username + "-" + password + "-" + returnurl);

            var user = _userService.GetItemByUserName(form.Username);
            //    if (user == null)
            //    {
            //        ViewBag.MessageError = "Email không tồn tại";
            //        return View();
            //    }
            //    else
            //    {
            //        if (user.Pass == Security.Encrypt(password))
            //        {
            //            if (user.Activity)
            //            {
            //                TempData["success"] = "Xin chào " + user.Name;
            //                string _token = Guid.NewGuid().ToString();
            //                var role = _roleService.GetByID(user.RoleID);
            //                if (role != null)
            //                {
            //                    HttpContext.SetValue(Cookies.DefaultLogin, _token, Cookies.ExpiresLogin, false);
            //                    //_ilogs.WriteLogsInfo(_token);
            //                    var claims = new List<Claim>
            //                    {
            //                        new Claim(ClaimTypes.Email, user.Email),
            //                        new Claim(ClaimTypes.Name, user.Name),
            //                        new Claim(ClaimTypes.Role, role.Code),
            //                        new Claim("RoleID", role.ID.ToString()),
            //                    };
            //                    var claimsIdentity = new ClaimsIdentity(claims, Cookies.DefaultLogin);
            //                    _ = new AuthenticationProperties
            //                    {
            //                        IsPersistent = true,
            //                        ExpiresUtc = DateTime.UtcNow.AddMinutes(Cookies.ExpiresLogin)
            //                    };
            //                    ClaimsPrincipal claim = new ClaimsPrincipal();
            //                    claim.AddIdentity(claimsIdentity);

            //                    CPLoginLogEntity login = new CPLoginLogEntity()
            //                    {
            //                        IP = HttpContext.Connection.RemoteIpAddress.ToString(),
            //                        Email = user.Email,
            //                        Token = _token,
            //                        Activity = true,
            //                        Created = DateTime.Now
            //                    };
            //                    var url = string.IsNullOrEmpty(returnurl) ? null : System.Net.WebUtility.UrlDecode(returnurl).Split('/');
            //                    _loginLogService.SetLogin(login);
            //                    return url == null
            //                        ? RedirectToAction("Index", "CPHome")
            //                        : RedirectToAction(url[1], url[0]);
            //                }
            //                else
            //                {
            //                    ViewBag.MessageError = "Bạn không có quyền hạn vào quản trị viện";
            //                    return View();
            //                }
            //            }
            //            else
            //            {
            //                ViewBag.MessageError = "Tài khoản của bạn đã bị khóa";
            //                return View();
            //            }

            //        }
            //        else
            //        {
            //            ViewBag.MessageError = "PassWord không chính xác";
            //            return View();
            //        }

            //}






            //if (service.IsNeedCheckCaptcha(form.Username) && CaptchaService.IsValidCaptcha(form.Captcha) == GlobalConstants.FALSE)
            //{
            //    throw new InvalidCaptchaException();
            //}

            //NGUOI_DUNG nguoiDung = service.FindUser(form.Username, form.Password);
            //if (nguoiDung == null || nguoiDung.LOAI_TAI_KHOAN != GlobalConstants.LOAI_TAI_KHOAN_ORIGIN)
            //{

            //    service.UpdateInvalidLogIn(form.Username);
            //    if (service.IsNeedCheckCaptcha(form.Username))
            //    {
            //        throw new InvalidAndEnableCaptchaException();
            //    }
            //    else
            //    {
            //        throw new InvalidUsernameOrPasswordException();
            //    }
            //}

            //service.RemoveInvalidLogIn(form.Username);

            // LoginResultForm res = SetDataToReturn(form, nguoiDung);
            //return res;
            LoginResultForm res = new LoginResultForm();
            res.UserName = user.UserName;
            res.Token = _accessTokenService.GetNewToken(user.ID,"",user.UserName);
            var pass= Security.Encrypt(form.Password);
            if (pass.Equals(user.Pass))
                return res;
            //string token = base.GetService<AccessTokenService>().GetNewToken(nguoiDung.NGUOI_DUNG_ID,
            //    GetHeader(GlobalConstants.HEADER_USER_AGENT), form.Username);
            return null;
        }

       // private LoginResultForm SetDataToReturn(AuthenticationForm form, NGUOI_DUNG nguoiDung)
       // {
            //Insert vao Database de quan ly

            //string token = base.GetService<AccessTokenService>().GetNewToken(nguoiDung.NGUOI_DUNG_ID,
            //    GetHeader(GlobalConstants.HEADER_USER_AGENT), form.Username);
            //LoginResultForm res = new LoginResultForm
            //{
            //    Token = token,
            //    VaiTro = nguoiDung.VAI_TRO
            //};
            //TRUONG_HOC truongHoc = GetService<TruongHocService>().Find(nguoiDung.MA_TRUONG_HOC);
            //if (truongHoc != null)
            //{
            //    res.CapHoc = truongHoc.CAP_HOC;
            //    res.TenDonVi = truongHoc.TEN_TRUONG_HOC;
            //    res.MaTruongHoc = truongHoc.MA_TRUONG_HOC;
            //    res.TenTruongHoc = truongHoc.TEN_TRUONG_HOC;
            //}

            ////check loại sở/phòng
            //if (nguoiDung.MA_DON_VI != null)
            //{
            //    DON_VI donvi = GetService<DonViService>().Find(nguoiDung.MA_DON_VI);
            //    if (donvi != null)
            //    {
            //        res.LoaiDonVi = donvi.LOAI_DON_VI.GetValueOrDefault();
            //        res.TenDonVi = donvi.TEN_DON_VI;
            //        //MaDonVi
            //        res.MaDonVi = donvi.MA_DON_VI;
            //    }
            //}
            //else
            //{
            //    if (truongHoc != null && truongHoc.MA_DON_VI != null)
            //    {
            //        DON_VI donvi = GetService<DonViService>().Find(truongHoc.MA_DON_VI);

            //        if (donvi != null)
            //        {
            //            res.LoaiDonVi = donvi.LOAI_DON_VI.GetValueOrDefault();
            //            res.TenDonVi = donvi.TEN_DON_VI;
            //            if (donvi.LOAI_DON_VI == 1)
            //            {
            //                //MaDonVi
            //                res.MaDonVi = donvi.MA_DON_VI;
            //            }
            //            else if (donvi.LOAI_DON_VI == 2)
            //            {
            //                //MaDonVi
            //                res.MaDonVi = donvi.MA_DON_VI_CHA;
            //            }
            //        }
            //    }

            //}
            //if (res.MaDonVi != null && GetService<ImageDonviService>().GetByMaDonVi(res.MaDonVi) != null)
            //{
            //    res.ImageLogo = Convert.ToBase64String(GetService<ImageDonviService>().GetByMaDonVi(res.MaDonVi).content);
            //}

            ////res.MaDonVi = nguoiDung.MA_DON_VI;
            //res.MaTinhThanh = nguoiDung.MA_TINH_THANH;
            //res.UserName = nguoiDung.TEN_DANG_NHAP;
            //res.LstMaChucNang = RoleManagementService.GetListMaChucNang(res.VaiTro);
            //return res;
          //  return null;
       // }

       
        //[SMEAuthorizeFilter]
        [HttpPost]
        public void LogOut()
        {
            //AccessTokenService accessTokenService = GetService<AccessTokenService>();
            //accessTokenService.Invalidate(GetHeader(GlobalConstants.HEADER_AUTHORIZATION)
            //    , GetHeader(GlobalConstants.HEADER_USERNAME));
        }

        //[SMEAuthorizeFilter]
        [HttpGet]
        public void KeepAlive()
        {
            //AccessTokenService accessTokenService = GetService<AccessTokenService>();
            //accessTokenService.KeepAlive(GetHeader(GlobalConstants.HEADER_AUTHORIZATION)
            //    , GetHeader(GlobalConstants.HEADER_USERNAME));
        }
    }
}
