using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly HistoryTransactionService _historyTransactionService;
        private readonly MappingEntity<NewsEntity, NewsViewModel> _mapping;
        private readonly ISession _session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DefaultConfigs _default { get; }

        public HomeController(FileProcess fileProcess, AccountService accountService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<DefaultConfigs> defaultvalue,
            CenterService centerService,
            StudentService studentService,
            NewsService newsService,
            NewsCategoryService newsCategoryService,
            ClassService classService,
            HistoryTransactionService historyTransactionService
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
            _historyTransactionService = historyTransactionService;
            _mapping = new MappingEntity<NewsEntity, NewsViewModel>();
        }

        public IActionResult Index(string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);
            var centerID = "";
            ViewBag.Student = student;
            if (student != null)
                ViewBag.AllCenters = student.Centers.Where(t => _centerService.GetItemByID(t).ExpireDate >= DateTime.Now).Select(t => _centerService.GetItemByID(t)).ToList();

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
            var data = _newsService.CreateQuery().Find(o => o.Type == "san-pham" && o.IsActive == true).Limit(6);

            List<NewsEntity> _data = new List<NewsEntity>();
            foreach (var item in data.ToList())
            {
                if ((item.Targets != null && item.Targets.Find(x => x == centerID) != null) || item.CenterID == centerID)
                    _data.Add(item);
            }

            ViewBag.List_Courses = _data.ToList();
            //var avatar = student != null && !string.IsNullOrEmpty(student.Avatar) ? student.Avatar : _default.defaultAvatar;
            //HttpContext.Session.SetString("userAvatar", avatar);
            return View();
        }

        public JsonResult DetailProduct(string ID)
        {
            var detail_product = _newsService.CreateQuery().Find(o => o.ID.Equals(ID) && o.Type == "san-pham").FirstOrDefault();
            ViewBag.Title = detail_product?.Title;
            return Json(detail_product);
        }

        public IActionResult Profile()
        {
            string _studentid = User.Claims.GetClaimByType("UserID") != null ? User.Claims.GetClaimByType("UserID").Value.ToString() : "0";
            var account = _studentService.GetItemByID(_studentid);
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

        #region getCourse
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

        public IActionResult Payment(string ID)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);

            var inforProduct = _newsService.CreateQuery().Find(o => o.ID.Equals(ID) && o.Type.Equals("san-pham")).FirstOrDefault();

            NewsViewModel DataResponse =
               _mapping.AutoOrtherType(inforProduct, new NewsViewModel()
               {
                   //ParentName = t.Name == null ? null : _serviceNewCate.CreateQuery().Find(x => x.ID == t.ParentID).ToList()
                   ClassName = inforProduct.ClassID == null || inforProduct.ClassID == "0" || inforProduct.ClassID == "" ? null : _classService.GetItemByID(inforProduct.ClassID).Name,
                   CenterName = inforProduct.CenterID == null || inforProduct.CenterID == "0" || inforProduct.CenterID == "" ? null : _centerService.GetItemByID(inforProduct.CenterID).Name

               });

            ViewBag.Product = DataResponse;
            return View();
        }

        //[HttpPost]
        //public async Task<JsonResult> Payment(string ID)
        //{
        //    string _studentid = User.Claims.GetClaimByType("UserID").Value;
        //    var student = _studentService.GetItemByID(_studentid);

        //    var inforProduct = _newsService.CreateQuery().Find(o => o.ID.Equals(ID) && o.Type.Equals("san-pham")).FirstOrDefault();

        //    var Dataresponse = new Dictionary<string, object>()
        //    {
        //        {"Product",inforProduct },
        //        {"Student",student},
        //        {"Success","OK" }
        //    };
        //    return Json(Dataresponse);
        //}

        public JsonResult JoinClass(string ID, string basis)
        {
            string _studentid = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(_studentid);

            var product = _newsService.GetItemByID(ID);
            var ClassID = product.ClassID;

            if (student.Centers.Where(o => o == ClassID) == null)
                student.Centers.Add(ClassID);

            _studentService.CreateQuery().ReplaceOne(o => o.ID == student.ID, student);

            //tạo lịch sử giao dịch
            var historyTransaction = new HistoryTransactionEntity();
            historyTransaction.StudentID = student.ID;
            historyTransaction.NewsID = ID;//mua san pham nao
            historyTransaction.Price = product.Discount == 0 ? product.Price : product.Discount;
            historyTransaction.CenterID = _centerService.GetItemByCode(basis).ID;
            historyTransaction.DayBuy = DateTime.UtcNow;

            _historyTransactionService.CreateOrUpdate(historyTransaction);
            //hết tạo lịch sử giao dịch

            if (string.IsNullOrEmpty(ClassID))
                return Json(new { error = "Lớp không tồn tại" });
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
                return Json(new { error = "Lớp không tồn tại" });
            //var student = _studentService.GetItemByID(StudentID);
            if (student == null)
                return Json(new { error = "Học viên không tồn tại" });

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
                return Json(new { data = @class, msg = "Học viên đã có trong lớp" });
            }
            if (_studentService.JoinClass(ClassID, student.ID, @class.Center) > 0)
            {

                return Json(new { data = @class, msg = "Học viên đã được thêm vào lớp" });
            }
            return Json(new { error = "Có lỗi, vui lòng thực hiện lại" });
        }
        #endregion
    }
}
