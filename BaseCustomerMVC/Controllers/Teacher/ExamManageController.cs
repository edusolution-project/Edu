using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using BaseCustomerEntity.Database;
using MongoDB.Driver;
using BaseCustomerMVC.Models;
using Core_v2.Globals;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using BaseCustomerEntity.Globals;

namespace BaseCustomerMVC.Controllers.Teacher
{
    public class ExamManageController : TeacherController
    {
        private readonly CenterService _centerService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly GradeService _gradeService;
        private readonly ExamQuestionArchiveService _examQuestionArchiveService;
        private readonly ClassService _classService;
        private readonly CourseLessonService _courseLessonService;
        private readonly LessonPartService _lessonPartService;
        private readonly LessonPartQuestionService _lessonPartQuestionService;
        private readonly LessonPartQuestionExtensionServie _lessonPartQuestionExtensionServie;
        private readonly LessonPartAnswerService _lessonPartAnswerService;
        private readonly LessonPartAnswerExtensionService _lessonPartAnswerExtensionService;
        //private readonly ExamProcessService _examProcessService;
        private readonly LessonService _lessonService;
        private readonly LessonHelper _lessonHelper;
        private readonly CloneLessonPartQuestionService _cloneLessonPartQuestionService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly LessonPartExtensionService _lessonPartExtensionService;
        private readonly CloneLessonPartService _cloneLessonPartService;
        private readonly CloneLessonPartAnswerService _cloneLessonPartAnswerService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly TeacherHelper _teacherHelper;

        //extenstion
        private readonly LessonExamService _lessonExamService;
        private readonly CloneLessonPartExtensionService _cloneLessonPartExtensionService;
        private readonly CloneLessonPartQuestionExtensionService _cloneLessonPartQuestionExtensionService;
        private readonly CloneLessonPartAnswerExtensionService _cloneLessonPartAnswerExtensionService;
        private readonly TagsService _tagsService;
        private readonly MatrixExamService _matrixExamService;
        private readonly ManageExamService _manageExamService;

        private readonly MappingEntity<ExamQuestionArchiveEntity, ExamQuestionArchiveViewModel> _examQuestionArchiveViewMapping = new MappingEntity<ExamQuestionArchiveEntity, ExamQuestionArchiveViewModel>();
        private readonly MappingEntity<LessonPartQuestionExtensionEntity, LessonPartQuestionEntity> _lessonPartQuestionExtensionMapping = new MappingEntity<LessonPartQuestionExtensionEntity, LessonPartQuestionEntity>();
        private readonly MappingEntity<LessonExamEntity, LessonEntity> _lessonMapping = new MappingEntity<LessonExamEntity, LessonEntity>();
        public ExamManageController(
            CenterService centerService
            , TeacherService teacherService
            , SubjectService subjectService
            , GradeService gradeService
            , ExamQuestionArchiveService examQuestionArchiveService
            , ClassService classService
            , CourseLessonService courseLessonService
            , LessonPartService lessonPartService
            , LessonPartQuestionService lessonPartQuestionService
            , LessonPartQuestionExtensionServie lessonPartQuestionExtensionServie
            , LessonPartAnswerService lessonPartAnswerService
            , LessonPartAnswerExtensionService lessonPartAnswerExtensionService
            //, ExamProcessService examProcessService
            , LessonService lessonService
            , LessonHelper lessonHelper
            , CloneLessonPartQuestionService cloneLessonPartQuestionService
            , ClassSubjectService classSubjectService
            , LessonPartExtensionService lessonPartExtensionService
            , CloneLessonPartService cloneLessonPartService
            , CloneLessonPartAnswerService cloneLessonPartAnswerService

            //extentsion
            , LessonExamService lessonExamService
            , CloneLessonPartExtensionService cloneLessonPartExtensionService
            , CloneLessonPartQuestionExtensionService cloneLessonPartQuestionExtensionService
            , CloneLessonPartAnswerExtensionService cloneLessonPartAnswerExtensionService
            , LessonScheduleService lessonScheduleService
            , TagsService tagsService
            , MatrixExamService matrixExamService
            , ManageExamService manageExamService
            , TeacherHelper teacherHelper
            )
        {
            _centerService = centerService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _gradeService = gradeService;
            _examQuestionArchiveService = examQuestionArchiveService;
            _classService = classService;
            _courseLessonService = courseLessonService;
            _lessonPartService = lessonPartService;
            _lessonPartQuestionService = lessonPartQuestionService;
            _lessonPartQuestionExtensionServie = lessonPartQuestionExtensionServie;
            _lessonPartAnswerService = lessonPartAnswerService;
            _lessonPartAnswerExtensionService = lessonPartAnswerExtensionService;
            //_examProcessService = examProcessService;
            _lessonService = lessonService;
            _lessonHelper = lessonHelper;
            _cloneLessonPartQuestionService = cloneLessonPartQuestionService;
            _classSubjectService = classSubjectService;
            _lessonPartExtensionService = lessonPartExtensionService;
            _cloneLessonPartService = cloneLessonPartService;
            _cloneLessonPartAnswerService = cloneLessonPartAnswerService;
            _lessonScheduleService = lessonScheduleService;

            //extenstion
            _lessonExamService = lessonExamService;
            _cloneLessonPartExtensionService = cloneLessonPartExtensionService;
            _cloneLessonPartQuestionExtensionService = cloneLessonPartQuestionExtensionService;
            _cloneLessonPartAnswerExtensionService = cloneLessonPartAnswerExtensionService;
            _tagsService = tagsService;
            _matrixExamService = matrixExamService;
            _manageExamService = manageExamService;
            _teacherHelper = teacherHelper;
        }
        public IActionResult Index(String basis)
        {
            //if (string.IsNullOrEmpty(basis)){
            //    return Json("Không tồn tại cơ sở.");
            //}

            //var center = _centerService.GetItemByCode(basis);

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

            if (teacher == null)
                return Redirect("/login");

            if (teacher != null && teacher.Subjects != null)
            {
                var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID) && t.IsActive).SortBy(t => t.Name).ToList();
                var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID)).ToList();
                //var classes = _classService.GetTeacherClassList(teacher.ID);
                var classes = _classService.CreateQuery().Find(t => t.Members.Any(o => o.TeacherID == UserID && o.Type == ClassMemberType.TEACHER)).ToList();
                ViewBag.Grades = grade;
                ViewBag.Subjects = subject;
                ViewBag.CurrentUser = teacher;
                ViewBag.ClassList = classes;
            }

            return View();
        }

        public IActionResult StorageQuestion(String basis, String ID)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

                if (teacher == null)
                    return Redirect("/login");

                if (teacher != null && teacher.Subjects != null)
                {
                    var currentExamArchive = _examQuestionArchiveService.GetItemByID(ID);
                    var subject = _subjectService.CreateQuery().Find(t => teacher.Subjects.Contains(t.ID) && t.IsActive && t.ID == currentExamArchive.SubjectID).SortBy(t => t.Name).ToList();
                    var grade = _gradeService.CreateQuery().Find(t => teacher.Subjects.Contains(t.SubjectID) && t.SubjectID == currentExamArchive.SubjectID).ToList();
                    //var classes = _classService.GetTeacherClassList(teacher.ID);
                    ViewBag.Grades = grade;
                    ViewBag.Subjects = subject;
                    ViewBag.CurrentUser = teacher;
                    ViewBag.CurrentExamArchive = currentExamArchive;
                    //ViewBag.ClassList = classes;
                }
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public JsonResult Getlist(DefaultModel model, String basis, String SubjectID, String GradeID)
        {
            try
            {
                var filter = new List<FilterDefinition<ExamQuestionArchiveEntity>>();

                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var center = _centerService.GetItemByCode(basis);
                if (center == null)
                {
                    return Json("Cơ sở không đúng.");
                }
                filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => o.CenterID == center.ID));

                var teacher = _teacherService.GetItemByID(UserID);

                if (teacher == null)
                {
                    return new JsonResult(new Dictionary<string, object>
                    {
                        { "Error", "Bạn không được quyền thực hiện thao tác này"}
                    });
                }
                if (!string.IsNullOrEmpty(SubjectID))
                {
                    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => o.SubjectID == SubjectID));
                }
                else
                {
                    //lọc các môn được phân công
                    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
                }
                if (!string.IsNullOrEmpty(GradeID))
                {
                    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => o.GradeID == GradeID));
                }

                if (!string.IsNullOrEmpty(model.SearchText))
                    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Text("\"" + model.SearchText + "\""));
                filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(x => x.CreateUser.Equals(teacher.ID)));

                var data = (filter.Count > 0 ? _examQuestionArchiveService.Collection.Find(Builders<ExamQuestionArchiveEntity>.Filter.And(filter)) : _examQuestionArchiveService.GetAll()).SortByDescending(t => t.ID);
                model.TotalRecord = data.CountDocuments();

                var response = new Dictionary<string, object>
                {
                };

                if (model.PageIndex < 0 || model.PageIndex * model.PageSize > model.TotalRecord)
                {
                    response = new Dictionary<string, object>
                    {
                        { "Model", model }
                    };
                }
                else
                {
                    var DataResponse = data == null || model.TotalRecord <= 0 //|| model.TotalRecord < model.PageSize
                        ? data
                        : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize);
                    //var DataResponse = data;

                    var rsp = DataResponse.ToList().Select(o =>
                        _examQuestionArchiveViewMapping.Auto(o, new ExamQuestionArchiveViewModel()
                        {
                            //SkillName = _skillService.GetItemByID(o.SkillID)?.Name,
                            GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                            SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                            TotalQuestion = (int)_lessonPartExtensionService.CreateQuery().Find(x => x.ExamQuestionArchiveID == o.ID).CountDocuments()
                        })).ToList();

                    response = new Dictionary<string, object>
                    {
                        { "Data", rsp},
                        { "Model", model }
                    };
                }

                return Json(response);
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public JsonResult GetExamManage(DefaultModel model,String basis)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var tc = _teacherService.GetItemByID(UserID);
                var center = _centerService.GetItemByCode(basis);

                if(center == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Data", null },
                        {"Msg","Cơ sở không tồn tại." }
                    });
                }

                if(tc == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Data", null },
                        {"Msg","Bạn không có quyền sử dụng chức năng này." }
                    });
                }
                List<ManageExamEntity> listData = new List<ManageExamEntity>();
                if(_teacherHelper.HasRole(UserID,center.ID, "head-teacher"))
                {
                    var data = _manageExamService.GetItemsByTeacherAndCenter("",center.ID);
                    var newData = (from d in data.ToList()
                                  let user = _teacherService.GetItemByID(d.CreateUser)
                                  let listExam = _lessonExamService.GetItemsByManageExamID(d.ID)
                                  let totalExam = listExam.Count()
                                  select new ManageExamViewModel(d)
                                  {
                                      UserName = user == null ? "" : user.FullName,
                                      TotalExam = totalExam,
                                      ListExam = listExam
                                  }).ToList();
                    listData.AddRange(newData);
                }
                else
                {
                    var data = _manageExamService.GetItemsByTeacherAndCenter(UserID,center.ID);
                    listData.AddRange(data.ToList());
                }
                return Json(new Dictionary<String, Object> 
                {
                    {"Status",true },
                    {"Data", listData },
                    {"Msg","" }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Status",false },
                    {"Data", null },
                    {"Msg",ex.Message }
                });
            }
        }

        public JsonResult GetMatrixExams(DefaultModel model,String basis)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var listMatrixs = _matrixExamService.CreateQuery().Find(x => x.CreateUser == UserID).ToList();
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg","" },
                    {"Data",listMatrixs }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Msg",ex.Message },
                    {"Data",null }
                });
            }
        }

        #region kho de
        public JsonResult CreateOrUpdate(String basis, ExamQuestionArchiveEntity item)
        {
            try
            {
                var center = _centerService.GetItemByCode(basis);
                if (center == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Error","Bạn không có quyền ở cơ sở này." }
                    });
                }

                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();

                if (teacher == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Error","Thông tin tài khoản không đúng." }
                    });
                }

                if (String.IsNullOrEmpty(item.ID)) //them moi
                {
                    item.CreateUser = teacher.ID;
                    item.CenterID = center.ID;
                    item.Created = DateTime.Now;
                    item.Updated = DateTime.Now;

                    _examQuestionArchiveService.Save(item);

                    var data = new ExamQuestionArchiveViewModel()
                    {
                        ID = item.ID,
                        Name = item.Name,
                        CreateUser = item.CreateUser,
                        GradeName = _gradeService.GetItemByID(item.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(item.SubjectID)?.Name
                    };

                    return Json(new Dictionary<String, Object>
                    {
                        {"Success","Thêm thành công" },
                        {"Data",data }
                    });
                }
                else //sua thong tin
                {
                    var oldItem = _examQuestionArchiveService.GetItemByID(item.ID);
                    oldItem.Updated = DateTime.Now;
                    oldItem.IsShare = item.IsShare;
                    oldItem.SubjectID = item.SubjectID;
                    oldItem.GradeID = item.GradeID;
                    oldItem.Description = item.Description;

                    _examQuestionArchiveService.Save(oldItem);

                    var data = new ExamQuestionArchiveViewModel()
                    {
                        ID = oldItem.ID,
                        Name = oldItem.Name,
                        CreateUser = oldItem.CreateUser,
                        GradeName = _gradeService.GetItemByID(oldItem.GradeID)?.Name,
                        SubjectName = _subjectService.GetItemByID(oldItem.SubjectID)?.Name
                    };
                    return Json(new Dictionary<String, Object>
                    {
                        {"Success","Thêm thành công" },
                        {"Data",data }
                    });
                }

                //return Json("");
            }
            catch (Exception e)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Error",e.Message }
                });
            }
        }

        public JsonResult DeleteExamQuestionArchive(List<String> IDs)
        {
            try
            {
                if (IDs.Count() == 0)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Msg","Kho de khong hop le." },
                        {"IDs",new List<String>() }
                    });
                }
                foreach (var item in IDs)
                {
                    _examQuestionArchiveService.Remove(item);
                    //var filterQ = new List<FilterDefinition<LessonPartQuestionExtensionEntity>>();
                    //filterQ.Add(Builders<LessonPartQuestionExtensionEntity>.Filter.Where(x => x.ParentID == item));
                    var question = _lessonPartQuestionExtensionServie.CreateQuery().Find(x => x.ParentID == item).Project(x => x.ID).ToList();
                    _lessonPartQuestionExtensionServie.CreateQuery().DeleteMany(Builders<LessonPartQuestionExtensionEntity>.Filter.Where(x => x.ParentID == item));
                    _lessonPartAnswerExtensionService.CreateQuery().DeleteMany(Builders<LessonPartAnswerExtensionEntity>.Filter.Where(x => question.Contains(x.ParentID)));
                }

                return Json(new Dictionary<String, Object>
                    {
                        {"Msg","Kho de khong hop le." },
                        {"IDs",IDs }
                    });
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<String, Object>
                    {
                        {"Msg",ex.Message },
                        {"IDs",new List<String>() }
                    });
            }
        }
        #endregion

        #region Quiz
        //public JsonResult CreateOrUpdateLessonPart(String basis, List<String> IDs, List<String> Types, String ID, String GradeID)
        public JsonResult CreateOrUpdateLessonPart(String basis, List<LessonPartExtensionEntity> lessonPartExtentsions, String ID, String GradeID)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var examQuestionArchive = _examQuestionArchiveService.GetItemByID(ID);
                var center = _centerService.GetItemByCode(basis);
                if (center == null) return Json("Cơ sở không tồn tại.");
                if (examQuestionArchive == null) return Json("Kho đề không tồn tại");
                if (String.IsNullOrEmpty(UserID) || _teacherService.GetItemByID(UserID) == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Msg","Tài khoản không tồn tại." },
                        {"Data", new List<LessonPartExtensionEntity>() }
                    });
                }
                //if (item == null)
                //{
                if (lessonPartExtentsions.Count() == 0)
                {
                    return Json(new Dictionary<String, Object>
                {
                    {"Msg","ID.count() == 0" },
                    {"Data", new List<LessonPartExtensionEntity>() }
                });
                }
                var listPart = new List<LessonPartExtensionEntity>();
                for (Int32 i = 0; i < lessonPartExtentsions.Count(); i++)
                {
                    var lessonPartExtension = lessonPartExtentsions.ElementAtOrDefault(i);
                    
                    var lessonPart = _lessonPartService.GetItemByID(lessonPartExtension.OriginID);
                    var lessonPartQuestion = _lessonPartQuestionService.GetByPartID(lessonPartExtension.OriginID);
                    var lessonpart = new LessonPartExtensionEntity
                    {
                        OriginID = lessonPart.ID,
                        Title = lessonPart.Title,
                        Timer = lessonPart.Timer,
                        Description = lessonPart.Description,
                        Type = lessonPart.Type,
                        Point = lessonPart.Point,
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        //Order = lessonPart.Order,
                        Media = lessonPart.Media,
                        LevelPart = lessonPartExtension.LevelPart,
                        ExamQuestionArchiveID = ID,
                        GradeID = GradeID,
                        SubjectID = examQuestionArchive.SubjectID,
                    };

                    if(!String.IsNullOrEmpty(lessonPartExtension.Tags))
                    {
                        var listTags = lessonPartExtension.Tags.Split(';');
                        var tag = "";
                        foreach(var t in listTags)
                        {
                            var codeT= t.ConvertUnicodeToCode("-", true);
                            Int32 pos = 0;
                            while (_tagsService.GetItemByCode(codeT) != null)
                            {
                                pos++;
                                codeT += ("-" + pos);
                            }
                            var newTag = new TagsEntity
                            {
                                Name = t,
                                Code = codeT,
                                ExamQuestionArchiveID = ID,
                                CenterCode = center.Code,
                                CreateUser = UserID
                            };
                            _tagsService.Save(newTag);
                            tag += $"{codeT}; ";
                        }
                        lessonpart.Tags = tag;
                    }    

                    _lessonPartExtensionService.Save(lessonpart);
                    listPart.Add(lessonpart);

                    var _newItem = (from l in lessonPartQuestion
                                    select new LessonPartQuestionExtensionEntity(l)
                                    {
                                        OriginID = l.ID,
                                        Media = l.Media,
                                        //TypeQuestion = itemType,
                                        ParentID = lessonpart.ID,
                                        //ExamQuestionArchiveID = ID
                                    });
                    var newItem = new List<LessonPartQuestionExtensionEntity>();
                    foreach (var ni in _newItem)
                    {
                        _lessonPartQuestionExtensionServie.CreateQuery().InsertOne(ni);
                        newItem.Add(ni);
                    }
                    
                    var lessonPartAns = _lessonPartAnswerService.GetByQuestionIDs(lessonPartQuestion.Select(x => x.ID).ToList());
                    var newAns = (from a in lessonPartAns.ToList()
                                  let q = newItem.Where(x => x.OriginID == a.ParentID).FirstOrDefault()
                                  where q != null
                                  select new LessonPartAnswerExtensionEntity(a)
                                  {
                                      Created = DateTime.Now,
                                      Updated = DateTime.Now,
                                      CreateUser = UserID,
                                      OriginID = a.ID,
                                      ParentID = q.ID,
                                  }).ToList();
                    _lessonPartAnswerExtensionService.CreateQuery().InsertMany(newAns);
                }

                var newListPart = (from lp in listPart
                                   let tagName = _tagsService.GetNamesByCodes(lp.Tags)
                                   select new LessonPartExtensionViewModel(lp)
                                   {
                                       TagsName = tagName
                                   }).ToList();

                return Json(new Dictionary<String, Object>
                {
                    {"Msg","Thêm thành công" },
                    {"Data",newListPart }
                });
                //}
                //else
                //{
                //    item.Created = DateTime.Now;
                //    item.Updated = DateTime.Now;
                //    item.CreateUser = UserID;
                //    _lessonPartQuestionExtensionServie.Save(item);
                //    return Json("Them thanh cong");
                //}
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Msg",ex.Message },
                    {"Data", new List<LessonPartExtensionEntity>() }
                });
            }
        }

        public JsonResult GetListPart(DefaultModel model, String basis, Int32 LevelPart, String ID, String GradeID = "", String SubjectID = "")
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var teacher = _teacherService.GetItemByID(UserID);
                if (teacher == null)
                {
                    return Json(new Dictionary<string, object>
                {
                    {"Error","Tai khoan khong ton tai." }
                });
                }
                var filter = new List<FilterDefinition<LessonPartExtensionEntity>>();
                if (LevelPart != 0)
                {
                    filter.Add(Builders<LessonPartExtensionEntity>.Filter.Where(x => x.LevelPart == LevelPart));
                }

                if (!String.IsNullOrEmpty(ID))
                {
                    filter.Add(Builders<LessonPartExtensionEntity>.Filter.Where(x => x.ExamQuestionArchiveID == ID));
                }

                if (!String.IsNullOrEmpty(SubjectID) && !String.IsNullOrEmpty(GradeID))
                {
                    filter.Add(Builders<LessonPartExtensionEntity>.Filter.Where(x => x.GradeID.Equals(GradeID) && x.SubjectID.Equals(SubjectID)));
                }

                var data = (filter.Count > 0 ? _lessonPartExtensionService.Collection.Find(Builders<LessonPartExtensionEntity>.Filter.And(filter)) : _lessonPartExtensionService.GetAll()).SortByDescending(t => t.ID);
                model.TotalRecord = data.CountDocuments();

                var response = new Dictionary<string, object>
                {
                };

                if (model.PageIndex < 0 || model.PageIndex * model.PageSize > model.TotalRecord)
                {
                    response = new Dictionary<string, object>
                    {
                        { "Model", model }
                    };
                }
                else
                {
                    var DataResponse = data == null || model.TotalRecord <= 0 //|| model.TotalRecord < model.PageSize
                        ? data
                        : data.Skip((model.PageIndex) * model.PageSize).Limit(model.PageSize);
                    //var DataResponse = data;

                    var listPart = (from lp in DataResponse.ToList()
                                    let tagname = _tagsService.GetNamesByCodes(lp.Tags)
                                    select new LessonPartExtensionViewModel(lp)
                                    {
                                        TagsName = tagname
                                    }).ToList();
                    response = new Dictionary<string, object>
                    {
                        { "Data", listPart},
                        { "Model", model }
                    };
                }
                return Json(response);
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<string, object>
                {
                    {"Error",ex.Message }
                });
            }
        }

        public JsonResult ChangeStatus(String basis, String ID, Int32 NewType)
        {
            try
            {
                if (String.IsNullOrEmpty(ID))
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"ID",null },
                        {"NewType",null },
                        {"Error","Khong tim thay cau hoi tuong ung" }
                    });
                }

                //var question = _lessonPartQuestionExtensionServie.GetItemByID(ID);
                var lessonpart = _lessonPartExtensionService.GetItemByID(ID);
                if (lessonpart == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"ID",null },
                        {"NewType",null },
                        {"Error","Khong tim thay cau hoi tuong ung" }
                    });
                }

                lessonpart.LevelPart = NewType;
                lessonpart.Updated = DateTime.Now;
                _lessonPartExtensionService.Save(lessonpart);
                return Json(new Dictionary<String, Object>
                {
                    {"ID",lessonpart.ID },
                    {"NewType",lessonpart.LevelPart }
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult ChangeTimer(String basis, String ID, Int32 Timer)
        {
            try
            {
                if (String.IsNullOrEmpty(ID))
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"ID",null },
                        {"NewType",null },
                        {"Error","Khong tim thay cau hoi tuong ung" }
                    });
                }

                //var question = _lessonPartQuestionExtensionServie.GetItemByID(ID);
                var lessonpart = _lessonPartExtensionService.GetItemByID(ID);
                if (lessonpart == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"ID",null },
                        {"NewType",null },
                        {"Error","Khong tim thay cau hoi tuong ung" }
                    });
                }

                lessonpart.Timer = Timer;
                lessonpart.Updated = DateTime.Now;
                _lessonPartExtensionService.Save(lessonpart);
                return Json(new Dictionary<String, Object>
                {
                    {"ID",lessonpart.ID },
                    {"Timer",lessonpart.Timer }
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult GetDetailQuestion(String ID)
        {
            try
            {
                var ans = _lessonPartAnswerExtensionService.GetItemByParentID(ID);
                if (ans == null || ans.Count() == 0)
                {
                    return Json("Khong co noi dung");
                }

                return Json(new Dictionary<String, Object>
                {
                    {"Data",ans.ToList() }
                });
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult DeleteLessonPart(String ID)
        {
            try
            {
                _lessonPartExtensionService.Remove(ID);
                var questionIDs = _lessonPartQuestionExtensionServie.GetIDsByPartID(ID);
                _lessonPartQuestionExtensionServie.CreateQuery().DeleteMany(x => questionIDs.Contains(x.ID));
                _lessonPartAnswerExtensionService.CreateQuery().DeleteMany(x => questionIDs.Contains(x.ParentID));
                return Json(new Dictionary<String, Object>
                {
                    {"Data",ID },
                    {"Status" ,true}
                });
            }
            catch (Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Data",ex.Message },
                    {"Status" ,true}
                });
            }
        }
        #endregion

        #region CreateExam
        public async Task<JsonResult> CreateOrUpdateExam(DefaultModel model, String basis, ExamProcessViewModel item,List<MatrixExamViewModel> matrixExams, Boolean isNew)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (String.IsNullOrEmpty(UserID) || _teacherService.GetItemByID(UserID) == null)
                {
                    return Json("Tai khoan khong ton tai.");
                }

                var centerID = _centerService.GetItemByCode(basis).ID;

                //Lưu format đề
                var matrixExam = new MatrixExamEntity
                {
                    Name = matrixExams.FirstOrDefault().Name,
                    ExamQuestionArchiveID = item.ExamQuestionArchiveID,
                    Created = DateTime.UtcNow,
                    CreateUser = UserID,
                    Center = centerID,
                };

                for (var i = 0; i < matrixExams.Count; i++)
                {
                    var f = matrixExams.ElementAtOrDefault(i);
                    f.Know.Total = f.Know.Theory + f.Know.Exercise;
                    f.Understanding.Total = f.Understanding.Theory + f.Understanding.Exercise;
                    f.Manipulate.Total = f.Manipulate.Theory + f.Manipulate.Exercise;
                    f.ManipulateHighly.Total = f.ManipulateHighly.Theory + f.ManipulateHighly.Exercise;
                    var detail = new DetailMatrixExam
                    {
                        Level = f.Level,
                        Order = i,
                        Tags = f.Tags,
                        Know = f.Know,
                        Understanding = f.Understanding,
                        Manipulate = f.Manipulate,
                        ManipulateHighly = f.ManipulateHighly,
                        
                    };
                    matrixExam.DetailFormat.Add(detail);
                }

                _matrixExamService.Save(matrixExam);

                if (item.Template == EXAM_TYPE.ISLECTURE) // luyen tap thi khong tao de truoc
                {
                    return Json("");
                }
                else //kiem tra thi tao de truoc
                {
                    if (isNew)
                    {
                        List<String> listLessonExamIDs = new List<string>();
                        List<LessonExamEntity> lessonExams = new List<LessonExamEntity>();
                        List<Int32> listIndex = new List<Int32>();
                        var rd = new Random();
                        var index = rd.Next(100, 999);

                        //taoj mowis kif kierm tra
                        var manageexam = new ManageExamEntity
                        {
                            Name = item.Title,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                            CreateUser = UserID,
                            Center = centerID,
                            Limtit = item.Limit,
                            Timer = item.Timer
                        };
                        _manageExamService.Save(manageexam);

                        //tạo mã đề, gán cho từng kì thi
                        for (Int32 i = 0; i < item.TotalExam; i++)
                        {
                            if (listIndex.Contains(index))
                            {
                                index = rd.Next(100, 999);
                            }
                            var codeExam = index;
                            listIndex.Add(index);

                            var lessonExam = new LessonExamEntity
                            {
                                TemplateType = 2,
                                Timer = item.Timer,
                                CreateUser = UserID,
                                Title = item.Title,
                                Created = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                                Limit = item.Limit,
                                Multiple = item.Multiple,//he so
                                Etype = item.Etype,
                                ChapterID = "0",
                                IsParentCourse = true,
                                //MatrixExamID = matrixExam.ID,
                                CodeExam = index.ToString(),
                                ManageExamID = manageexam.ID
                            };
                            _lessonExamService.Save(lessonExam);
                            //lessonExams.Add(lessonExam);
                            listLessonExamIDs.Add(lessonExam.ID);
                            //var str = RenderExam(lessonExam,matrixExam,item.ExamQuestionArchiveID,UserID).Result;
                        }
                    }
                    return Json("");
                }

                //

                //var @class = _classService.GetItemByID(item.TargetClasses.FirstOrDefault());
                //var classsbj = _classSubjectService.CreateQuery().Find(x => x.ClassID == @class.ID && x.TypeClass == CLASSSUBJECT_TYPE.EXAM).FirstOrDefault();

                //if (isNew)
                //{
                //    item.Created = DateTime.Now;
                //    item.Updated = DateTime.Now;
                //    item.CreateUser = UserID;

                //    var _lesson = new LessonEntity
                //    {
                //        TemplateType = 2,
                //        Timer = item.Timer,
                //        CreateUser = UserID,
                //        Title = item.Title,
                //        Created = DateTime.Now,
                //        Updated = DateTime.Now,
                //        Limit = item.Limit,
                //        Multiple = item.Multiple,
                //        Etype = item.Etype,
                //        ClassID = @class.ID,
                //        ClassSubjectID = classsbj.ID,
                //        ChapterID = "0",
                //        IsParentCourse = true
                //    };
                //    _lessonService.Save(_lesson);

                //    var lessonShechude = new LessonScheduleEntity
                //    {
                //        ClassID = _lesson.ClassID,
                //        ClassSubjectID = _lesson.ClassSubjectID,
                //        IsHideAnswer = true,
                //        IsActive = _lesson.IsActive,
                //        LessonID = _lesson.ID,
                //        TeacherID = UserID,
                //        Type = CLASSSUBJECT_TYPE.EXAM
                //    };
                //    _lessonScheduleService.Save(lessonShechude);

                //    //render exam
                //    for (Int32 i = 0; i < item.TotalExam; i++)
                //    {
                //        var lesson = new LessonExtensionEntity
                //        {
                //            TemplateType = 2,
                //            Timer = item.Timer,
                //            CreateUser = UserID,
                //            Title = item.Title,
                //            Created = DateTime.Now,
                //            Updated = DateTime.Now,
                //            Limit = item.Limit,
                //            Multiple = item.Multiple,
                //            Etype = item.Etype,
                //            ClassID = _lesson.ClassID,
                //            ClassSubjectID = _lesson.ClassSubjectID,
                //            ChapterID = "0",
                //            IsParentCourse = true,
                //            LessonID = _lesson.ID,
                //            Code = new Random().Next(1, 100).ToString()
                //        };
                //        _lessonExamService.Save(lesson);

                //        item.LessonID = lesson.ID;
                //        //_examProcessService.Save(item);
                //        var msg = ProcessCreateExam(item, UserID, lesson).Result;
                //    }

                //    //tạo đề khác tương tự

                //}
                //return Json("");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        private async Task<String> RenderExam(LessonExamEntity lessonExam, MatrixExamEntity matrixExam, String ID,String UserID)
        {
            //lay danh sach cac lessonpart trong ngan hang cau hoi
            var lessonParts = _lessonPartExtensionService.GetItemsByExamQuestionArchiveID(ID);
            if (lessonParts.Count() == 0)
            {
                return "";
            }

            var lessonpartIDs = lessonParts.Select(x => x.ID).ToList();
            var listQuestions = _lessonPartQuestionExtensionServie.CreateQuery().Find(x => lessonpartIDs.Contains(x.ParentID));
            var listQuestionIDs = listQuestions.Project(x => x.ID).ToList();
            var listAns = _lessonPartAnswerExtensionService.CreateQuery().Find(x => listQuestionIDs.Contains(x.ParentID));

            //var listPartKnow = new List<LessonPartExtensionEntity>();
            //var listPartUnderstanding = new List<LessonPartExtensionEntity>();
            //var listPartManipulate = new List<LessonPartExtensionEntity>();
            //var listPartManipulateHighly = new List<LessonPartExtensionEntity>();
            var newListParts = new List<LessonPartExtensionEntity>();
            var newListQuestion = new List<LessonPartQuestionExtensionEntity>();
            var newListAns = new List<LessonPartAnswerExtensionEntity>();
            foreach (var item in matrixExam.DetailFormat)
            {
                var _listPartKnow = lessonParts.Where(x => x.LevelPart == LEVELPART.KNOW && item.Tags.Contains(x.Tags)).Take(item.Total); // 
                var _listPartUnderstanding = lessonParts.Where(x => x.LevelPart == LEVELPART.UNDERSTANDING && item.Tags.Contains(x.Tags)).Take(item.Total);
                var _listPartManipulate = lessonParts.Where(x => x.LevelPart == LEVELPART.MANIPULATE && item.Tags.Contains(x.Tags)).Take(item.Total);
                var _listPartManipulateHighly = lessonParts.Where(x => x.LevelPart == LEVELPART.MANIPULATEHIGHLY && item.Tags.Contains(x.Tags)).Take(item.Total);

                newListParts.AddRange(_listPartKnow);
                newListParts.AddRange(_listPartUnderstanding);
                newListParts.AddRange(_listPartManipulate);
                newListParts.AddRange(_listPartManipulateHighly);
            }

            if (newListParts.Count() > 0)
                newListQuestion.AddRange(GetQuestion(newListParts, listQuestions.ToList()).Result);
            if (newListQuestion.Count() > 0)
                newListAns.AddRange(GetAnswer(newListQuestion, listAns.ToList()).Result);

            foreach (var part in newListParts)
            {
                var clonelessonpart = new CloneLessonPartExtensionEntity
                {
                    OriginID = part.ID,
                    //ParentID = lesson.ID,
                    Title = part.Title,
                    Timer = part.Timer,
                    Description = part.Description,
                    Type = part.Type,
                    Point = part.Point,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Order = part.Order,
                    //ClassID = lesson.ClassID,
                    //ClassSubjectID = lesson.ClassSubjectID,
                    TeacherID = UserID,
                    Media = part.Media == null ? null : new Media { Created = part.Media.Created, Extension = part.Media.Extension, Name = part.Media.Name, OriginalName = part.Media.OriginalName, Path = part.Media.Path, Size = part.Media.Size }
                };

                _cloneLessonPartExtensionService.Save(clonelessonpart);
                SetQuestion(UserID, null, newListQuestion, newListAns, part, clonelessonpart);
            }

            return "";
        }

        /// <summary>
        /// lesson => tạo nơi chứa thông tin bài kiểm tra
        /// </summary>
        /// <param name="item"></param>
        /// <param name="UserID"></param>
        /// <param name="lesson"></param>
        /// <returns></returns>
        public async Task<String> ProcessCreateExam(ExamProcessViewModel item, String UserID, LessonExamEntity lesson)
        {
            // danh sach lessonpart trong kho de
            var lessonPart = _lessonPartExtensionService.GetItemsByExamQuestionArchiveID(item.ExamQuestionArchiveID);
            if (lessonPart.Count() == 0)
            {
                return "Không tìm thấy câu hỏi trong kho đề, kiểm tra lại";
            }

            var lessonpartIDs = lessonPart.Select(x => x.ID).ToList();
            var listQuestions = _lessonPartQuestionExtensionServie.CreateQuery().Find(x => lessonpartIDs.Contains(x.ParentID));
            var listQuestionIDs = listQuestions.Project(x => x.ID).ToList();
            var listAns = _lessonPartAnswerExtensionService.CreateQuery().Find(x => listQuestionIDs.Contains(x.ParentID));

            var listPartKnow = lessonPart.Where(x => x.LevelPart == LEVELPART.KNOW); // 
            var listPartUnderstanding = lessonPart.Where(x => x.LevelPart == LEVELPART.UNDERSTANDING);
            var listPartManipulate = lessonPart.Where(x => x.LevelPart == LEVELPART.MANIPULATE);
            var listPartManipulateHighly = lessonPart.Where(x => x.LevelPart == LEVELPART.MANIPULATEHIGHLY);

            //for (var j = 0; j < item.TotalExam; j++)
            //{
            var newListPart = new List<LessonPartExtensionEntity>();
            var newListQuestion = new List<LessonPartQuestionExtensionEntity>();
            var newListAns = new List<LessonPartAnswerExtensionEntity>();
            //var rd = new Random();

            var listpartKnow = GetLessonPart(item.KnowQuestion, listPartKnow.ToList()).Result;
            var listpartUnderstanding = GetLessonPart(item.UnderstandQuestion, listPartUnderstanding.ToList()).Result;
            var listpartManipulate = GetLessonPart(item.ManipulateQuestion, listPartManipulate.ToList()).Result;
            var listpartManipulatehighly = GetLessonPart(item.ManipulateHighlyQuestion, listPartManipulate.ToList()).Result;

            newListPart.AddRange(listpartKnow);
            newListPart.AddRange(listpartUnderstanding);
            newListPart.AddRange(listpartManipulate);
            newListPart.AddRange(listpartManipulatehighly);

            if (newListPart.Count() > 0)
                newListQuestion.AddRange(GetQuestion(newListPart, listQuestions.ToList()).Result);
            if (newListQuestion.Count() > 0)
                newListAns.AddRange(GetAnswer(newListQuestion, listAns.ToList()).Result);
            
            foreach (var part in newListPart)
            {
                var clonelessonpart = new CloneLessonPartExtensionEntity
                {
                    OriginID = part.ID,
                    ParentID = lesson.ID,
                    Title = part.Title,
                    Timer = part.Timer,
                    Description = part.Description,
                    Type = part.Type,
                    Point = part.Point,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Order = part.Order,
                    ClassID = lesson.ClassID,
                    ClassSubjectID = lesson.ClassSubjectID,
                    TeacherID = UserID,
                    Media = part.Media == null ? null : new Media { Created = part.Media.Created, Extension = part.Media.Extension, Name = part.Media.Name, OriginalName = part.Media.OriginalName, Path = part.Media.Path, Size = part.Media.Size }
                };

                _cloneLessonPartExtensionService.Save(clonelessonpart);
                SetQuestion(UserID, lesson, newListQuestion, newListAns, part, clonelessonpart);
            }
            //}
            return "";
        }

        public async Task<List<LessonPartExtensionEntity>> GetLessonPart(Int32 Total, List<LessonPartExtensionEntity> Parts)
        {
            var arrayIndex = new List<Int32>();
            List<LessonPartExtensionEntity> listPart = new List<LessonPartExtensionEntity>();
            for (var i = 0; i < Total; i++)
            {
                Random rd = new Random();
                Int32 index = 0;
                do
                {
                    index = rd.Next(0, Parts.Count());
                    if (!arrayIndex.Contains(index))
                    {
                        arrayIndex.Add(index);
                        break;
                    }
                }
                while (arrayIndex.Contains(index));
            }

            foreach (var index in arrayIndex)
            {
                var item = Parts.ElementAtOrDefault(index);
                if (item != null)
                {
                    listPart.Add(item);
                }
            }
            return listPart;
        }

        private void SetQuestion(string UserID, LessonExamEntity lesson, List<LessonPartQuestionExtensionEntity> newListQuestion, List<LessonPartAnswerExtensionEntity> newListAns, LessonPartExtensionEntity part, CloneLessonPartExtensionEntity clonelessonpart)
        {
            foreach (var quiz in newListQuestion.Where(x => x.ParentID == part.ID))
            {
                var clonelessonpartquestion = new CloneLessonPartQuestionExtensionEntity
                {
                    OriginID = quiz.ID,
                    Content = quiz.Content,
                    CreateUser = UserID,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Point = quiz.Point,
                    Order = quiz.Order,
                    Description = quiz.Description,
                    Media = quiz.Media,
                    ParentID = clonelessonpart.ID,
                    ClassID = lesson == null ? "" : lesson.ClassID,
                    ClassSubjectID = lesson == null ? "" : lesson.ClassSubjectID,
                    LessonID = lesson == null ? "" : lesson.ID
                };

                _cloneLessonPartQuestionExtensionService.Save(clonelessonpartquestion);
                if (part.Type == "QUIZ3")
                {
                    String a = "";
                }
                SetAnswer(UserID, lesson, newListAns, quiz, clonelessonpartquestion);
            }
        }

        private async Task<List<LessonPartQuestionExtensionEntity>> GetQuestion(List<LessonPartExtensionEntity> lessonPartExtensions, List<LessonPartQuestionExtensionEntity> lessonPartQuestions)
        {
            List<LessonPartQuestionExtensionEntity> questions = new List<LessonPartQuestionExtensionEntity>();
            foreach (var item in lessonPartExtensions)
            {
                var question = lessonPartQuestions.Where(x => x.ParentID == item.ID).ToList();
                if (question == null) continue;
                questions.AddRange(question);
            }
            return questions;
        }

        private void SetAnswer(string UserID, LessonExamEntity lesson, List<LessonPartAnswerExtensionEntity> newListAns, LessonPartQuestionExtensionEntity quiz, CloneLessonPartQuestionExtensionEntity clonelessonpartquestion)
        {
            foreach (var ans in newListAns.Where(x => x.ParentID == quiz.ID))
            {
                var cloneAns = new CloneLessonPartAnswerExtensionEntity
                {
                    OriginID = ans.ID,
                    ParentID = clonelessonpartquestion.ID,
                    Content = ans.Content,
                    IsCorrect = ans.IsCorrect,
                    CreateUser = UserID,
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    Media = ans.Media == null ? null : new Media { Name = ans.Media.Name, OriginalName = ans.Media.OriginalName, Created = ans.Media.Created, Extension = ans.Media.Extension, Path = ans.Media.Path, Size = ans.Media.Size },
                    Order = ans.Order,
                    //CourseID = ans.CourseID,
                    TeacherID = UserID,
                    ClassID = lesson == null ? "" : lesson.ClassID,
                    ClassSubjectID = lesson == null ? "" : lesson.ClassSubjectID
                };

                _cloneLessonPartAnswerExtensionService.Save(cloneAns);
            }
        }

        private async Task<List<LessonPartAnswerExtensionEntity>> GetAnswer(List<LessonPartQuestionExtensionEntity> lessonPartQuestions, List<LessonPartAnswerExtensionEntity> lessonPartAnswers)
        {
            List<LessonPartAnswerExtensionEntity> answers = new List<LessonPartAnswerExtensionEntity>();
            foreach (var question in lessonPartQuestions)
            {
                var ans = lessonPartAnswers.Where(x => x.ParentID == question.ID).ToList();
                if (ans == null) continue;
                answers.AddRange(ans);
            }
            return answers;
        }

        public JsonResult CreateOrUpdateMatrix(String basis)
        {
            try
            {
                return Json(new Dictionary<String, Object> {

                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {

                });
            }
        }

        public JsonResult RemoveMatrix(String basis,String ID)
        {
            try
            {
                var matrix = _matrixExamService.GetItemByID(ID);
                if(matrix == null)
                    return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Msg","Không tìm thấy thông tin" }
                });

                _matrixExamService.Remove(ID);

                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg","Đã xoá" }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Msg",ex.Message }
                });
            }
        }

        #endregion

        #region Option
        public JsonResult GetOptionFormatExam(String basis, String ID)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var exam = _examQuestionArchiveService.GetItemByID(ID);
                if (exam == null)
                {
                    return Json(new Dictionary<String, Object>
                    {
                        {"Status",false },
                        {"Msg","" },
                        {"Data",null }
                    });
                }
                var tags = _tagsService.GetItemByUserAndCenter(basis,UserID);
                return Json(new Dictionary<String, Object>
                {
                    {"Status",true },
                    {"Msg","" },
                    {"Data",tags }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object>
                {
                    {"Status",false },
                    {"Msg",ex.Message },
                    {"Data",null }
                });
            }    
        }
        #endregion
    }

    #region class other
    public class DataQuestion
    {
        [JsonProperty("ID")]
        public String ID { get; set; }
        [JsonProperty("Type")]
        public Int32 Type { get; set; }
    }
    #endregion
}
