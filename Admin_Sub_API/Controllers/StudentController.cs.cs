
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
using System.IO;
using OfficeOpenXml;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class StudentController : ControllerBase
    {
        StudentEntityService _studentService;
        CPUserSubService _userService;
        AccessTokenService _accessTokenService;
        public StudentController(
        CPUserSubService userService, AccessTokenService accessTokenService, StudentEntityService studentEntityService
       )
        {
            _userService = userService;
            _accessTokenService = accessTokenService;
            _studentService = studentEntityService;
        }

        [HttpPost]
        [Obsolete]
        public Task<BaseResponse<StudentEntity>> getList([FromBody]SeachForm seachForm)
        {
            return _studentService.getList(seachForm);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody]StudentEntity item)
        {
            var id = item.ID;
            if (!string.IsNullOrEmpty(item.UserCreate))
            {
                try
                {
                    var userItem = _userService.GetItemByUserName(item.UserCreate);
                    item.UserNameManager = userItem.UserNameManager;
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }
            if (_studentService.GetItemByUserName(item.StudentId) != null && string.IsNullOrEmpty(item.ID))
                return BadRequest("Tên tài khoản đã có trong hệ thống");


            // item.Pass = "123";
            // item.Pass = Security.Encrypt(item.Pass);
            await _studentService.AddAsync(item);



            if (_userService.GetItemByUserName(item.StudentId) != null && string.IsNullOrEmpty(item.ID))
                return BadRequest("Tên tài khoản đã có trong hệ thống");
            CPUserSubEntity userSub;
            if (string.IsNullOrEmpty(id))
            {
                userSub = new CPUserSubEntity();
                userSub.Pass = "123";
                userSub.Pass = Security.Encrypt(userSub.Pass);
                userSub.UserName = item.StudentId;
                userSub.FullName = item.FullName;
                userSub.RoleID = "HOCVIEN";
                userSub.UserNameManager = item.UserNameManager;
                userSub.Activity = item.Activity;

            }
            else
            {
                userSub = _userService.GetItemByUserName(item.StudentId);
                userSub.FullName = item.FullName;
                userSub.Activity = item.Activity;

            }
            await _userService.AddAsync(userSub);
            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]StudentEntity item)
        {
            await _studentService.RemoveAsync(item.ID);

            //xoa bang subuser

            var userItem = _userService.GetItemByUserName(item.StudentId);

            await _userService.RemoveAsync(userItem.ID);

            return NoContent();

        }

        [HttpPost]
        public async Task<IActionResult> ImportExcel()
        {
            var httpRequest = HttpContext.Request.Form.Files[0];
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await httpRequest.CopyToAsync(stream);
                using (ExcelPackage package = new ExcelPackage(stream))
                {
                    ExcelWorksheet workSheet = package.Workbook.Worksheets["sheet_nghiepnc"];
                    int totalRows = workSheet.Dimension.Rows;

                    List<StudentEntity> teacherList = new List<StudentEntity>();
                    for (int i = 2; i <= totalRows; i++)
                    {
                        teacherList.Add(new StudentEntity
                        {
                            StudentId = workSheet.Cells[i, 1].Value.ToString(),
                            FullName = workSheet.Cells[i, 2].Value.ToString() + workSheet.Cells[i, 3].Value.ToString(),
                            Classes = workSheet.Cells[i, 3].Value.ToString()
                        });
                    }
                    //var listPro = _propertyService.GetItemByParentID(item.ID);
                    //if (listPro != null)
                    //{
                    //    _propertyService.RemoveRange(listPro.Select(o => o.ID).ToList());
                    //}
                    //_studentService.RemoveRange(teacherList);
                    await _studentService.AddRangeAsync(teacherList);


                }
            }


            return NoContent();

        }
    }
}
