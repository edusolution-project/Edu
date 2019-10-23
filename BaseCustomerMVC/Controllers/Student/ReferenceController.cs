using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using BaseCustomerMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class ReferenceController : StudentController
    {
        private readonly TeacherService _teacherService;
        private readonly ClassService _classService;
        private readonly FileProcess _fileProcess;
        private readonly ReferenceService _referenceService;

        public ReferenceController(
            TeacherService teacherService,
            ClassService classService,
            FileProcess fileProcess,
            ReferenceService referenceService
            )
        {
            _teacherService = teacherService;
            _classService = classService;
            _referenceService = referenceService;
            _fileProcess = fileProcess;
        }

        public JsonResult GetList(ReferenceEntity entity, DefaultModel defaultModel, string TeacherID)
        {
            if (entity != null)
            {
                var UserID = TeacherID;
                var result = new List<ReferenceEntity>();
                switch (entity.Range)
                {
                    case REF_RANGE.TEACHER:
                        result = _referenceService.CreateQuery().Find(t => t.Range == REF_RANGE.TEACHER && t.OwnerID == UserID).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                    case REF_RANGE.CLASS:
                        result = _referenceService.CreateQuery().Find(t => t.OwnerID == UserID && (t.Range == REF_RANGE.TEACHER) || (t.Range == REF_RANGE.CLASS && t.Target == entity.Target)).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                    //case REF_RANGE.ALL:
                    default:
                        result = _referenceService.CreateQuery().Find(t => t.Range == REF_RANGE.ALL).Skip(defaultModel.PageSize * defaultModel.PageIndex).Limit(defaultModel.PageSize).ToList();
                        break;
                }
                return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", result },
                });
            }
            return new JsonResult(new Dictionary<string, object>
                {
                    { "Data", null},
                });
        }
    }
}
