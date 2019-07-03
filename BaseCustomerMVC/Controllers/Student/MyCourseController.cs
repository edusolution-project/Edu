using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Core_v2.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class MyCourseController : StudentController
    {
        public MyCourseController()
        {

        }

        //thông tin sắp xếp theo , mon => listCourse => listChapter/ listLesson

            /// chia nhỏ api 
            /// 1 => getlistCourse chỉ lấy danh sách giáo trình group theo môn và cấp độ
            /// 2 => getlistLesson chỉ lấy danh sách bài giảng group theo giáo trình (save lại localstore nếu được)
            /// 3 => getDetailsLesson => bài tập LessonViewModel + phục vụ cả làm bài tập chấm điểm.

        /// <summary>
        /// lấy danh sách bài giảng
        /// </summary>
        /// <returns></returns>
        public Task<JsonResult> GetListCourse(DefaultModel model)
        {
            try
            {
                

                return Task.FromResult(new JsonResult("ok"));
            }
            catch(Exception ex)
            {
                return Task.FromResult(new JsonResult(ex));
            }
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StudentCalendar()
        {
            return View();
        }

    }
}
