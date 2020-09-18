﻿using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class HomeController : StudentController
    {
        private StudentService _studentService;
        private readonly FileProcess _fileProcess;
        private readonly AccountService _accountService;
        private readonly CenterService _centerService;
        private readonly NewsService _newsService;
        private readonly NewsCategoryService _newsCategoryService;
        private readonly ClassService _classService;
        private readonly TransactionService _transactionService;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mapping;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultConfigs _default { get; }

        private string host;

        public HomeController(FileProcess fileProcess, AccountService accountService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<DefaultConfigs> defaultvalue,
            CenterService centerService,
            StudentService studentService,
            NewsService newsService,
            NewsCategoryService newsCategoryService,
            ClassService classService,
            TransactionService historyTransactionService,
            IConfiguration iConfig
            )
        {
            _studentService = studentService;
            _accountService = accountService;
            _centerService = centerService;
            _fileProcess = fileProcess;
            _newsCategoryService = newsCategoryService;
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
            _default = defaultvalue.Value;
            _newsService = newsService;
            _classService = classService;
            _transactionService = historyTransactionService;
            _mapping = new MappingEntity<NewsEntity, NewsViewModel>();
            host = iConfig.GetValue<string>("SysConfig:Domain");
        }

        public IActionResult Index(string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            var centerID = "";
            ViewBag.Student = student;
            if (student != null)
                ViewBag.AllCenters = student.Centers?.Where(t => _centerService.GetItemByID(t).ExpireDate >= DateTime.Now).Select(t => _centerService.GetItemByID(t))?.ToList();

            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                {
                    ViewBag.Center = center;
                    centerID = center.ID;
                }
            }

            //var category = _newsCategoryService.GetItemByCode("san-pham");

            //var data = _newsService.CreateQuery().Find(o => o.CenterID == centerID && o.Type == "san-pham" && o.IsActive == true ||o.IsPublic == true && o.IsActive==true).Limit(6);
            var data = _newsService.CreateQuery().Find(o => o.Type == "san-pham" && o.IsActive == true && o.Targets.Any(t => t == centerID)).Limit(6);

            ViewBag.List_Courses = data.ToList();

            return View();
        }

        public JsonResult DetailProduct(string ID)
        {
            var detail_product = _newsService.CreateQuery().Find(o => o.ID.Equals(ID) && o.Type == "san-pham").FirstOrDefault();
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            var dataResponse = _mapping.AutoOrtherType(detail_product, new NewsViewModel
            {
                SttPayment = _transactionService.CreateQuery().Find(x => x.StudentID == student.ID && x.NewsID == ID).FirstOrDefault() == null ? false : true
            });
            ViewBag.Title = detail_product?.Title;
            //return Json(detail_product);
            return Json(dataResponse);
        }

        public IActionResult Profile(string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var account = _studentService.GetItemByID(_studentid);
            if (account == null)
                return Redirect("/logout");

            if (!string.IsNullOrEmpty(basis))
            {
                var center = _centerService.GetItemByCode(basis);
                if (center != null)
                {
                    ViewBag.Center = center;
                }
            }
            ViewBag.avatar = account.Avatar ?? _default.defaultAvatar;
            _session.SetString("userAvatar", account.Avatar ?? _default.defaultAvatar);
            return View(account);
        }

        public JsonResult GetProfile()
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
                var account = _studentService.GetItemByID(_studentid);
                var avatar = account.Avatar ?? _default.defaultAvatar;
                _session.SetString("userAvatar", avatar);
                ViewBag.avatar = avatar;
                account.Avatar = avatar;
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.SUCCESS,
                    StatusDesc = "OK",
                    Data = account
                });
            }
            catch (Exception e)
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = e.Message,
                });
            }
        }

        [HttpPost]
        public IActionResult Profile(StudentEntity entity)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var acc = _studentService.GetItemByID(_studentid);
            acc.FullName = entity.FullName;
            acc.Phone = entity.Phone;
            acc.Skype = entity.Skype;
            _studentService.CreateOrUpdate(acc);
            ViewBag.avatar = acc.Avatar ?? _default.defaultAvatar;
            ViewBag.Description = "Lưu thành công!";
            return View(acc);
        }

        public JsonResult SaveProfile(StudentEntity entity, IFormFile fileUpload)
        {
            try
            {
                string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
                var account = _studentService.GetItemByID(_studentid);
                account.FullName = entity.FullName;
                account.Phone = entity.Phone;
                account.Skype = entity.Skype;
                if (fileUpload != null)
                {
                    var pathImage = _fileProcess.SaveMediaAsync(fileUpload, fileUpload.FileName, "Avatar").Result;
                    account.Avatar = pathImage;
                    _session.SetString("userAvatar", account.Avatar);
                }

                _studentService.CreateOrUpdate(account);
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.SUCCESS,
                    StatusDesc = "SaveOk",
                    Data = account
                });
            }
            catch (Exception e)
            {
                return Json(new ReturnJsonModel
                {
                    StatusCode = ReturnStatus.ERROR,
                    StatusDesc = e.Message,
                });
            }
        }

        [HttpPost]
        public JsonResult UploadPhoto(IFormFile fileUpload)
        {
            var pathImage = _fileProcess.SaveMediaAsync(fileUpload, fileUpload.FileName, "Avatar").Result;
            // Cap nhat vao truong avartar
            string _studentId = User.Claims.GetClaimByType("UserID").Value;
            StudentEntity oldAcc = _studentService.GetItemByID(_studentId);
            oldAcc.Avatar = pathImage;
            _studentService.CreateOrUpdate(oldAcc);
            _session.SetString("userAvatar", oldAcc.Avatar);
            //return Content(pathImage);

            return Json(new ReturnJsonModel
            {
                StatusCode = ReturnStatus.SUCCESS,
                StatusDesc = pathImage,
            });
        }

        [HttpPost]
        public JsonResult ChangePassword(string oldpass, string newpass, string retypepass)
        {
            /*
             * 1. Kiem tra mat khau cu co khop hay khong
             * 2. kiem tra mat khau moi va xac nhan mat khau co khop nhau hay khong
             * 2.1 Kiem tra mat khau phai co it nhat 6 ki tu
             * 3. Luu mat khat moi vao CSDL
             * 4. Thong bao thanh cong cho don vi
             */

            if (string.IsNullOrEmpty(oldpass))
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Password is not correct" }
                    });
            }

            if (string.IsNullOrEmpty(newpass) || newpass.Length < 6)
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Password's length must be over 6 " }
                    });
            }

            if (!newpass.Equals(retypepass))
            {
                return new JsonResult(
                new Dictionary<string, object>
                    {
                        { "Error", "Retype password is not matched" }
                    });
            }

            string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var acc = _studentService.GetItemByID(_studentid);

            AccountEntity user = _accountService.GetAccountByEmail(acc.Email);

            if (!Core_v2.Globals.Security.Encrypt(oldpass).Equals(user.PassWord))
            {
                return new JsonResult(
                new Dictionary<string, object>
                 {
                        { "Error", "Old password not correct" }
                 });
            }


            if (Core_v2.Globals.Security.Encrypt(newpass).Equals(user.PassWord))
            {
                return new JsonResult(
                new Dictionary<string, object>
                 {
                        { "Error", "New password can not be the same as old password" }
                 });
            }

            user.PassWord = Core_v2.Globals.Security.Encrypt(newpass);
            _accountService.CreateOrUpdate(user);
            return new JsonResult(
            new Dictionary<string, object>
                    {
                        { "Message", "Password Updated Successfully" }
                    });
        }

        #region getCourse and Payment
        [HttpPost]
        public JsonResult getListCourse()
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            var centerID = student.Centers[0];
            var filter = new List<FilterDefinition<NewsEntity>>();
            if (!string.IsNullOrEmpty(centerID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(o => o.CenterID == centerID));
            }

            if (!string.IsNullOrEmpty(centerID))
            {
                filter.Add(Builders<NewsEntity>.Filter.Where(o => o.Type == "san-pham"));
            }
            filter.Add(Builders<NewsEntity>.Filter.Where(o => o.IsActive == true));

            //var data = _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter));
            var data = _newsService.Collection.Find(Builders<NewsEntity>.Filter.And(filter));

            Dictionary<string, object> response = new Dictionary<string, object>()
            {
                {"Data",data.ToList() },
                {"Message","Success" }
            };
            return new JsonResult(response);
        }

        public IActionResult Payment(string ID, Boolean check = false)
        {
            if (check == false)
            {
                string _studentid = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(_studentid);

                var inforProduct = _newsService.CreateQuery().Find(o => o.ID.Equals(ID) && o.Type.Equals("san-pham")).FirstOrDefault();

                if (inforProduct == null) return Redirect("/");
                NewsViewModel DataResponse =
                   _mapping.AutoOrtherType(inforProduct, new NewsViewModel()
                   {
                       //ParentName = t.Name == null ? null : _serviceNewCate.CreateQuery().Find(x => x.ID == t.ParentID).ToList()
                       ClassName = inforProduct.ClassID == null || inforProduct.ClassID == "0" || inforProduct.ClassID == "" ? null : _classService.GetItemByID(inforProduct.ClassID).Name,
                       CenterName = inforProduct.CenterID == null || inforProduct.CenterID == "0" || inforProduct.CenterID == "" ? null : _centerService.GetItemByID(inforProduct.CenterID).Name
                   });

                ViewBag.Product = DataResponse;
                ViewBag.Student = student;
                return View();
            }
            else
            {
                string _studentid = User.Claims.GetClaimByType("UserID").Value;
                var student = _studentService.GetItemByID(_studentid);
                var transaction = _transactionService.CreateQuery().Find(x => x.StudentID == student.ID && x.NewsID == ID).FirstOrDefault();
                if (transaction != null && transaction.StatusPayment)
                {
                    var dataResponse = new Dictionary<string, object>()
                    {
                        {"msg","Gói học đã thanh toán thành công!" },
                        {"stt",true }
                    };
                    return Json(dataResponse);
                }
                else if (transaction != null && !transaction.StatusPayment)
                {
                    var dataResponse = new Dictionary<string, object>()
                    {
                        {"msg",$"Gói học thanh toán không thành công! </br> {transaction.MsgPayment}" },
                        {"stt",false }
                    };
                    return Json(dataResponse);
                }
                //else if (transaction != null)
                //{
                //    var dataResponse = new Dictionary<string, object>()
                //    {
                //        {"msg","Bạn đã mua khoá học này rồi!" },
                //        {"stt",false }
                //    };
                //    return Json(dataResponse);
                //}
                else
                {
                    return null;
                }
            }
        }

        //get ip client
        public string GetIPAddress()
        {
            var IPAddress = "";
            IPHostEntry Host = default(IPHostEntry);
            string Hostname = null;
            Hostname = System.Environment.MachineName;
            Host = Dns.GetHostEntry(Hostname);
            foreach (IPAddress IP in Host.AddressList)
            {
                if (IP.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    IPAddress = Convert.ToString(IP);
                }
            }
            return IPAddress;
        }

        [HttpPost]
        public JsonResult PaymentStatus(string basis, string ID = null, string Phone = null, string Name = null)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);

            var Address = Request.Form["Address"].ToString();

            var product = _newsService.GetItemByID(ID);
            var ClassID = product.ClassID;

            var Error = "";
            if (string.IsNullOrEmpty(ClassID))
                Error += "Lớp không tồn tại";
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
                Error += "Lớp không tồn tại";
            //var student = _studentService.GetItemByID(StudentID);
            if (student == null)
                Error += "Học viên không tồn tại";
            if (_studentService.IsStudentInClass(ClassID, student.ID))
            {
                Error += "Học viên đã có trong lớp.Giao dịch không thành công!";
                var dataresponse = new Dictionary<string, object>()
                {
                    {"Url","" },
                    {"Error", Error}
                };
                return Json(dataresponse);
            }
            else
            {
                //tạo lịch sử giao dịch
                var historyTransaction = new TransactionEntity();
                historyTransaction.StudentID = student.ID;
                historyTransaction.NewsID = ID;//mua san pham nao
                historyTransaction.Price = product.Discount;
                historyTransaction.CenterID = _centerService.GetItemByID(product.CenterID).ID;
                historyTransaction.DayBuy = DateTime.UtcNow;
                historyTransaction.StatusPayment = false;

                _transactionService.CreateOrUpdate(historyTransaction);

                if (historyTransaction.Price > 0)
                {
                    //string SECURE_SECRET = "6D0870CDE5F24F34F3915FB0045120DB";
                    string SECURE_SECRET = "11135E3DBA3E3D658B589E68C3C092E3";
                    // Khoi tao lop thu vien va gan gia tri cac tham so gui sang cong thanh toan
                    VPCRequest conn = new VPCRequest("https://onepay.vn/paygate/vpcpay.op");
                    conn.SetSecureSecret(SECURE_SECRET);
                    // Add the Digital Order Fields for the functionality you wish to use
                    // Core Transaction Fields
                    conn.AddDigitalOrderField("AgainLink", "https://onepay.vn/paygate/vpcpay.op");
                    conn.AddDigitalOrderField("Title", "Thanh toán khóa học " + product.Title);
                    conn.AddDigitalOrderField("vpc_Locale", "vn");//Chon ngon ngu hien thi tren cong thanh toan (vn/en)
                    conn.AddDigitalOrderField("vpc_Version", "2");
                    conn.AddDigitalOrderField("vpc_Command", "pay");
                    conn.AddDigitalOrderField("vpc_Merchant", "OP_EDUSO");
                    //conn.AddDigitalOrderField("vpc_Merchant", "TESTONEPAY");
                    //conn.AddDigitalOrderField("vpc_AccessCode", "6BEB2546");
                    conn.AddDigitalOrderField("vpc_AccessCode", "66VKMV0J");
                    conn.AddDigitalOrderField("vpc_MerchTxnRef", historyTransaction.ID); //ma giao dich
                    conn.AddDigitalOrderField("vpc_OrderInfo", historyTransaction.ID); //THong tin don hang
                    var price = product.Discount;
                    conn.AddDigitalOrderField("vpc_Amount", price.ToString() + "00");
                    conn.AddDigitalOrderField("vpc_ReturnURL", "https://" + host + processUrl(basis, "Transaction", "Home", new { ID, center = basis }));
                    //conn.AddDigitalOrderField("vpc_ReturnURL", "http://" + host + processUrl(basis, "Transaction", "Home", new { ID, center = basis }));
                    // Thong tin them ve khach hang. De trong neu khong co thong tin
                    conn.AddDigitalOrderField("vpc_Customer_Phone", Phone);
                    conn.AddDigitalOrderField("vpc_Customer_Id", student.ID);
                    conn.AddDigitalOrderField("vpc_Customer_Name", Name);
                    conn.AddDigitalOrderField("vpc_Customer_Address", Address);
                    // Dia chi IP cua khach hang
                    string IPAddress = GetIPAddress();
                    conn.AddDigitalOrderField("vpc_TicketNo", IPAddress);
                    // Chuyen huong trinh duyet sang cong thanh toan
                    String url = conn.Create3PartyQueryString();
                    //return Redirect(url);
                    var dataresponse = new Dictionary<string, object>()
                    {
                        {"Url",url },
                        {"Error","" }
                    };
                    return Json(dataresponse);
                }
                else //price = 0 => auto complete
                {
                    //TODO: Check here
                    var url = processUrl(basis, "Transaction", "Home", new { ID, vpc_TxnResponseCode = 0, vpc_MerchTxnRef = historyTransaction.ID, center = basis });
                    var dataresponse = new Dictionary<string, object>()
                    {
                        {"Url", url },
                        {"Error","" }
                    };
                    return Json(dataresponse);
                }

                //hết tạo lịch sử giao dịch
            }
        }

        public string JoinClass(string ID, string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            var product = _newsService.GetItemByID(ID);
            var center = _centerService.GetItemByCode(basis);
            var ClassID = product.ClassID;

            //thêm sinh viên vào cơ sở khác
            if (student.Centers.Where(o => o == center.ID) == null)
                student.Centers.Add(center.ID);
            _studentService.CreateQuery().ReplaceOne(o => o.ID == student.ID, student);
            //

            //if (string.IsNullOrEmpty(ClassID))
            //    return "Lớp không tồn tại";
            var @class = _classService.GetItemByID(ClassID);
            //if (@class == null)
            //    return "Lớp không tồn tại" ;
            ////var student = _studentService.GetItemByID(StudentID);
            //if (student == null)
            //    return "Học viên không tồn tại";

            if (student.JoinedClasses == null)
            {
                student.JoinedClasses = new List<string> { };
                _studentService.Save(student);//init JoinedClass;
            }
            if (student.Centers == null)
            {
                student.Centers = new List<string> { };
                _studentService.Save(student);//init Center;
            }
            if (_studentService.IsStudentInClass(ClassID, student.ID))
            {
                return "Học viên đã có trong lớp";
            }
            if (_studentService.JoinClass(ClassID, student.ID, @class.Center) > 0)
            {
                //if (student.Centers.Where(o => o == ClassID) == null)
                //    student.Centers.Add(ClassID);

                //_studentService.CreateQuery().ReplaceOne(o => o.ID == student.ID, student);
                return "Học viên đã được thêm vào lớp";
            }
            return "Có lỗi, vui lòng thực hiện lại";
        }

        [HttpGet]
        public IActionResult Transaction()
        {
            var vpc_TxnResponseCode = Request.Query["vpc_TxnResponseCode"].ToString();
            var idproduct = Request.Query["ID"].ToString();
            var basis = Request.Query["center"].ToString();
            var center = _centerService.GetItemByID(_newsService.GetItemByID(idproduct).CenterID)?.Code;
            var transactionID = Request.Query["vpc_MerchTxnRef"].ToString();
            if (vpc_TxnResponseCode.Equals("0"))
            {
                var vpc_TransactionNo = Request.Query["vpc_TransactionNo"].ToString();
                var historyTransaction = _transactionService.GetItemByID(transactionID);
                if (historyTransaction != null)
                {
                    historyTransaction.StatusPayment = true;
                    historyTransaction.DayPayment = DateTime.UtcNow;
                    historyTransaction.TradingID = vpc_TransactionNo;
                    historyTransaction.MsgPayment = ResponseCode(vpc_TransactionNo);
                    _transactionService.Save(historyTransaction);
                    JoinClass(idproduct, center);
                    //ViewBag.message = "Thanh toán thành công!";
                    //var redirec = $"/{center}/student/Course";
                    var redirec = "http://" + host + processUrl(basis, "Payment", "Home") + $"/{idproduct}";
                    return Redirect(redirec);
                }
                else
                {
                    historyTransaction.StatusPayment = false;
                    historyTransaction.DayPayment = DateTime.UtcNow;
                    historyTransaction.TradingID = vpc_TransactionNo;
                    historyTransaction.MsgPayment = ResponseCode(vpc_TransactionNo);
                    _transactionService.Save(historyTransaction);
                    //var redirec = "https://" + host + processUrl(basis, "Payment", "Home") + $"/{idproduct}";
                    var redirec = "http://" + host + processUrl(basis, "Payment", "Home") + $"/{idproduct}";
                    return Redirect(redirec);
                }
            }
            else
            {
                //ViewBag.message = "Thanh toán không thành công!";
                //var redirec = "https://" + host + processUrl(basis, "Payment", "Home") + $"/{idproduct}";
                var redirec = "http://" + host + processUrl(basis, "Payment", "Home") + $"/{idproduct}";
                return Redirect(redirec);
            }
            //return View();
        }
        #endregion

        protected string processUrl(string center, string act, string ctrl, Object param = null)
        {
            string url = Url.Action(act, ctrl, param);

            return $"/{center}{url}";
        }

        private string ResponseCode(string code)
        {
            var msg = "";
            switch (code)
            {
                case "0":
                    msg = "Giao dịch thành công";
                    break;
                case "1":
                    msg = "Giao dịch không thành công, " +
                        "Ngân hàng phát hành thẻ không cấp phép cho " +
                        "giao dịch hoặc thẻ chưa được kích hoạt dịch vụ " +
                        "thanh toán trên Internet.Vui lòng liên hệ ngân " +
                        "hàng theo số điện thoại sau mặt thẻ được hỗ " +
                        "trợ chi tiết.";
                    break;
                case "2":
                    msg = "Giao dịch không thành công, " +
                    "Ngân hàng phát hành thẻ từ chối cấp phép cho " +
                    "giao dịch. Vui lòng liên hệ ngân hàng theo số " +
                    "điện thoại sau mặt thẻ để biết chính xác nguyên " +
                    "nhân Ngân hàng từ chối.";
                    break;
                case "3":
                    msg = "Giao dịch không thành công, " +
                    "Cổng thanh toán không nhận được kết quả trả " +
                    "về từ ngân hàng phát hành thẻ. Vui lòng liên hệ " +
                    "với ngân hàng theo số điện thoại sau mặt thẻ " +
                    "để biết chính xác trạng thái giao dịch và thực " +
                    "hiện thanh toán lại";
                    break;
                case "4":
                    msg = "Giao dịch không thành công do thẻ hết hạn sử " +
                    "dụng hoặc nhập sai thông tin tháng / năm hết " +
                    "hạn của thẻ. Vui lòng kiểm tra lại thông tin và " +
                    "thanh toán lại";
                    break;
                case "5":
                    msg = "Giao dịch không thành công, " +
                    "Thẻ không đủ hạn mức hoặc tài khoản không " +
                    "đủ số dư để thanh toán.Vui lòng kiểm tra lại " +
                    "thông tin và thanh toán lạ";
                    break;
                case "6":
                    msg = "Giao dịch không thành công, " +
                    "Quá trình xử lý giao dịch phát sinh lỗi từ ngân " +
                    "hàng phát hành thẻ. Vui lòng liên hệ ngân hàng " +
                    "theo số điện thoại sau mặt thẻ được hỗ trợ chi " +
                    "tiết.";
                    break;
                case "7":
                    msg = "Giao dịch không thành công, " +
                    "Đã có lỗi phát sinh trong quá trình xử lý giao " +
                    "dịch.Vui lòng thực hiện thanh toán lại.";
                    break;
                case "8":
                    msg = "Giao dịch không thành công. Số thẻ không " +
                    "đúng.Vui lòng kiểm tra và thực hiện thanh toán " +
                    "lại";
                    break;
                case "9":
                    msg = "Giao dịch không thành công. Tên chủ thẻ " +
                    "không đúng. Vui lòng kiểm tra và thực hiện " +
                    "thanh toán lại";
                    break;
                case "10":
                    msg = "Giao dịch không thành công. Thẻ hết hạn/Thẻ " +
                    "bị khóa. Vui lòng kiểm tra và thực hiện thanh " +
                    "toán lại";
                    break;
                case "11":
                    msg = "Giao dịch không thành công. Thẻ chưa đăng ký " +
                    "sử dụng dịch vụ thanh toán trên Internet. Vui " +
                    "lòng liên hê ngân hàng theo số điện thoại sau " +
                    "mặt thẻ để được hỗ trợ.";
                    break;
                case "12":
                    msg = "Giao dịch không thành công. Ngày phát " +
                    "hành / Hết hạn không đúng.Vui lòng kiểm tra và " +
                    "thực hiện thanh toán lại";
                    break;
                case "13":
                    msg = "Giao dịch không thành công. thẻ/ tài khoản đã " +
                    "vượt quá hạn mức thanh toán. Vui lòng kiểm " +
                    "tra và thực hiện thanh toán lạ";
                    break;
                case "21":
                    msg = "Giao dịch không thành công. Số tiền không đủ " +
                    "để thanh toán.Vui lòng kiểm tra và thực hiện " +
                    "thanh toán lại";
                    break;
                case "22":
                    msg = "Giao dịch không thành công. Thông tin tài " +
                    "khoản không đúng.Vui lòng kiểm tra và thực " +
                    "hiện thanh toán lại ";
                    break;
                case "23":
                    msg = "Giao dịch không thành công. Tài khoản bị khóa. " +
                    "Vui lòng liên hê ngân hàng theo số điện thoại " +
                    "sau mặt thẻ để được hỗ trợ";
                    break;
                case "24":
                    msg = "Giao dịch không thành công. Thông tin thẻ " +
                    "không đúng. Vui lòng kiểm tra và thực hiện " +
                    "thanh toán lại";
                    break;
                case "25":
                    msg = "Giao dịch không thành công. OTP không đúng. " +
                    "Vui lòng kiểm tra và thực hiện thanh toán lại ";
                    break;
                case "253":
                    msg = "Giao dịch không thành công. Quá thời gian " +
                    "thanh toán. Vui lòng thực hiện thanh toán lại";
                    break;
                case "99":
                    msg = "Giao dịch không thành công. Người sử dụng " +
                    "hủy giao dịch";
                    break;
                case "B":
                case "b":
                    msg = "Giao dịch không thành công do không xác thực " +
                    "được 3D - Secure.Vui lòng liên hệ ngân hàng " +
                    "theo số điện thoại sau mặt thẻ được hỗ trợ chi " +
                    "tiết.";
                    break;
                case "E":
                case "e":
                    msg = "Giao dịch không thành công do nhập sai CSC " +
                    "(Card Security Card) hoặc ngân hàng từ chối " +
                    "cấp phép cho giao dịch. Vui lòng liên hệ ngân " +
                    "hàng theo số điện thoại sau mặt thẻ được hỗ " +
                    "trợ chi tiết.";
                    break;
                case "F":
                case "f":
                    msg = "Giao dịch không thành công do không xác thực " +
                    "được 3D - Secure.Vui lòng liên hệ ngân hàng " +
                    "theo số điện thoại sau mặt thẻ được hỗ trợ chi " +
                    "tiết.";
                    break;
                case "Z":
                case "z":
                    msg = "Giao dịch không thành công do vi phạm quy " +
                    "định của hệ thống. Vui lòng liên hệ với OnePAY " +
                    "để được hỗ trợ(Hotline: 1900 633 927)";
                    break;
                default:
                    msg = "Giao dịch không thành công. Vui lòng liên hệ " +
                        "với OnePAY để được hỗ trợ(Hotline: 1900 633 " +
                        "927).";
                    break;
            }
            return msg;
        }
    }
}
