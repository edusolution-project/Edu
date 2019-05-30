
using Newtonsoft.Json;
using System;
using NLog;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using BaseMongoDB.Database;

using Business.Dto.Form;
using BaseMVC.Globals;
using System.Net;
using BaseMVC.Models;
using System.Threading.Tasks;
//using OfficeOpenXml;
using System.IO;
using OfficeOpenXml;
using System.Linq;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class TeacherController : ControllerBase
    {



        TeacherService teacherService;
        AccessTokenService _accessTokenService;
        CPUserSubService userService;
        public TeacherController(
        TeacherService teacherService, AccessTokenService accessTokenService, CPUserSubService userService
       )
        {
            this.teacherService = teacherService;
            _accessTokenService = accessTokenService;
            this.userService = userService;
        }

        [HttpPost]
        public Task<BaseResponse<TeacherEntity>> getList([FromBody]SeachForm seachForm)
        {


            return teacherService.getList(seachForm);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody]TeacherEntity item)
        {
            var id = item.ID;
            if (!string.IsNullOrEmpty(item.UserCreate))
            {
                try {
                    var userItem = userService.GetItemByUserName(item.UserCreate);
                    item.UserNameManager = userItem.UserNameManager;
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
            if (teacherService.GetItemByUserName(item.TeacherId) != null && string.IsNullOrEmpty(item.ID))
                return BadRequest("Tên tài khoản đã có trong hệ thống");


            // item.Pass = "123";
            // item.Pass = Security.Encrypt(item.Pass);
            await teacherService.AddAsync(item);



            if (userService.GetItemByUserName(item.TeacherId) != null && string.IsNullOrEmpty(item.ID))
                return BadRequest("Tên tài khoản đã có trong hệ thống");
            CPUserSubEntity userSub;
            if (string.IsNullOrEmpty(id))
            {
                userSub = new CPUserSubEntity();
                userSub.Pass = "123";
                userSub.Pass = Security.Encrypt(userSub.Pass);
                userSub.UserName = item.TeacherId;
                userSub.FullName = item.FullName;
                userSub.RoleID = "GIAOVIEN";
                userSub.UserNameManager = item.UserNameManager;
                userSub.Activity = item.Activity;

            }
            else
            {
                userSub = userService.GetItemByUserName(item.TeacherId);
                userSub.FullName = item.FullName;
                userSub.Activity = item.Activity;

            }
            await userService.AddAsync(userSub);
            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]TeacherEntity item)
        {
            await teacherService.RemoveAsync(item.ID);

            //xoa bang subuser

            var userItem = userService.GetItemByUserName(item.TeacherId);

            await userService.RemoveAsync(userItem.ID);

            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel()
        {
            var httpRequest = HttpContext.Request.Form;
            var httpRequestFile = httpRequest.Files[0];

            var filePath = Path.GetTempFileName();

            var userCreate = httpRequest["userCreate"];

            var userItem = userService.GetItemByUserName(userCreate);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await httpRequestFile.CopyToAsync(stream);
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets["Sheet1"];
                         int totalRows = workSheet.Dimension.Rows;

                    List<TeacherEntity> teacherList = new List<TeacherEntity>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        teacherList.Add(new TeacherEntity
                        {
                            TeacherId = workSheet.Cells[i, 2].Value.ToString(),
                            FullName = workSheet.Cells[i, 3].Value.ToString() + " " + workSheet.Cells[i, 4].Value.ToString(),
                            Technique = workSheet.Cells[i, 6].Value.ToString(),
                            DateBorn = workSheet.Cells[i, 5].Value.ToString(),
                            UserCreate = "nghiepnc",
                            UserNameManager = userItem.UserNameManager,
                            Activity = true

                        });
                    }

                   
                   
                    var listTeacher = teacherService.getListByUserNameManager(userItem.UserNameManager);
                    if(listTeacher!=null)
                    {
                        teacherService.RemoveRange(listTeacher.Select(o => o.ID).ToList());
                    }
                    await teacherService.AddRangeAsync(teacherList);


                }
            }

           
            return NoContent();

        }

        //[HttpPost]
        //public IList<TeacherEntity> ImportCustomer()
        //{


        //  //  string rootFolder = _hostingEnvironment.WebRootPath;
        //    string fileName = @"ImportCustomers.xlsx";
        //    FileInfo file = new FileInfo(Path.Combine("", fileName));

        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        ExcelWorksheet workSheet = package.Workbook.Worksheets["Customer"];
        //        int totalRows = workSheet.Dimension.Rows;

        //        List<TeacherEntity> customerList = new List<TeacherEntity>();

        //        for (int i = 2; i <= totalRows; i++)
        //        {
        //            //customerList.Add(new TeacherEntity
        //            //{
        //            //    CustomerName = workSheet.Cells[i, 1].Value.ToString(),
        //            //    CustomerEmail = workSheet.Cells[i, 2].Value.ToString(),
        //            //    CustomerCountry = workSheet.Cells[i, 3].Value.ToString()
        //            //});
        //        }

        //       // _db.Customers.AddRange(customerList);
        //       // _db.SaveChanges();

        //        return customerList;
        //    }
        //}
    }
}
