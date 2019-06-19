
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
using MongoDB.Driver;
using AutoMapper;

namespace SME.API.Controllers
{

    //[ApiController]
    // [SMEExceptionFilter]
    public class CourseController : ControllerBase
    {



        CourseService courseService;
        ModCourseService_nghiepnc modCourseService;
        AccessTokenService _accessTokenService;
        CPUserSubService userService;
        ModSubjectService modSubjectService;
        ModGradeService modGradleService;
        ModProgramService modProgramService;
        ModLessonService modLessonService;
        ModChapterService modChapterService;
        CourseTeacherConfigService courseTeacherConfigService;
        public CourseController(
        CourseService courseService,
        AccessTokenService accessTokenService,
        CPUserSubService userService,
        ModCourseService_nghiepnc modCourseService,
        ModSubjectService modSubjectService,
        ModGradeService modGradleService,
        ModProgramService modProgramService,
        ModLessonService modLessonService,
        ModChapterService modChapterService,
        CourseTeacherConfigService courseTeacherConfigService
       )
        {
            this.courseService = courseService;
            _accessTokenService = accessTokenService;
            this.userService = userService;
            this.modCourseService = modCourseService;
            this.modSubjectService = modSubjectService;
            this.modGradleService = modGradleService;
            this.modProgramService = modProgramService;
            this.modLessonService = modLessonService;
            this.modChapterService = modChapterService;
            this.courseTeacherConfigService = courseTeacherConfigService;
        }

        [HttpPost]
        public Task<BaseResponse<CourseEntity>> getList([FromBody]SeachForm seachForm)
        {
            return courseService.getList(seachForm);
        }
        [HttpPost]
        public List<CourseTeacherConfigEntity> getListLesson([FromBody]SeachForm model)
        {
           
            // danh sach lesson theo courseID khong co chapter
            var listLessonNoChapter = modLessonService.getListByCourseIdNoChapter(model.courseID);
            var listCourseTeacherConfig = new List<CourseTeacherConfigEntity>();
            try
            {
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<ModLessonEntity,CourseTeacherConfigEntity>();
                });

                IMapper mapper = config.CreateMapper();
                 listCourseTeacherConfig= mapper.Map<List<ModLessonEntity>, List<CourseTeacherConfigEntity>>(listLessonNoChapter);


                var listConfigFromDB = courseTeacherConfigService.getListByCourseID(model.courseID);

                foreach (var item in listCourseTeacherConfig)
                {
                    item.TeacherID = model.teacherID;
                    foreach (var itemDb in listConfigFromDB)
                    {
                        if(item.ID == itemDb.ID)
                        {
                            item.endedDate = itemDb.endedDate;
                            item.createdDate = itemDb.createdDate;
                            item.IsOpen = itemDb.IsOpen;
                            break;
                        }
                       
                    }

                   
                }
            }
            catch(Exception ex)
            {
                string s = ex.Message;
            }
            //danh sach chapter theo courseid
            // var listChapter = modChapterService.getListByCourseId(model.courseID);

            //danh sach lesson theo courseid co chapter

            // var listLessonHaveChapter = modLessonService.getListByCourseIdHaveChapter(model.courseID);

            // noi lesson vao tung chapter
            ; return listCourseTeacherConfig;
        }


        [HttpPost]
        public List<ModCourseEntity_nghiepnc> getListModCourse([FromBody]SeachForm seachForm)
        {
            return modCourseService.getList(seachForm);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]CourseEntity item)
        {

            if (!string.IsNullOrEmpty(item.UserCreate))
            {
                try
                {
                    var userItem = userService.GetItemByUserName(item.UserCreate);
                    item.UserNameManager = userItem.UserNameManager;
                }
                catch (Exception ex)
                {
                    string s = ex.Message;
                }
            }

            await courseService.AddAsync(item);

            return NoContent();

        }


        [HttpPost]
        public async Task<IActionResult> ConfigCouse([FromBody]CourseTeacherConfigEntity item)
        {
          

            await courseTeacherConfigService.AddAsync(item);

            return NoContent();

        }

        [HttpPost]
        public List<ModSubjectEntity> getListModSubject()
        {
            var item = modSubjectService.CreateQuery().Find(o => o.IsActive == true).ToList();
            return item;
        }

        [HttpPost]
        public List<ModGradeEntity> getListModGradle()
        {
            var item = modGradleService.CreateQuery().Find(o => o.IsActive == true).ToList();
            return item;
        }

        [HttpPost]
        public List<ModProgramEntity> getListModProgram()
        {

            var item = modProgramService.CreateQuery().Find(o => o.IsActive == true).ToList();

            return item;
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]CourseEntity item)
        {
            await courseService.RemoveAsync(item.ID);

            return NoContent();

        }



    }
}
