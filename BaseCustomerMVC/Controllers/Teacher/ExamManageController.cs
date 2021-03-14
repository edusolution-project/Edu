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
        private readonly MainSubjectService _mainSubjectService;

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
            , MainSubjectService mainSubjectService
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
            _mainSubjectService = mainSubjectService;
        }
        public IActionResult Index(String basis)
        {
            //if (string.IsNullOrEmpty(basis)){
            //    return Json("Không tồn tại cơ sở.");
            //}

            //var center = _centerService.GetItemByCode(basis);

            var UserID = User.Claims.GetClaimByType("UserID").Value;
            var teacher = _teacherService.CreateQuery().Find(t => t.ID == UserID).SingleOrDefault();//: new TeacherEntity();
            var center = _centerService.GetItemByCode(basis);

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
                ViewBag.IsHeadTeacher = _teacherHelper.HasRole(UserID, center.ID, "head-teacher");
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
                //if (!string.IsNullOrEmpty(SubjectID))
                //{
                //    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => o.SubjectID == SubjectID));
                //}
                //else
                //{
                //    //lọc các môn được phân công
                //    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => teacher.Subjects.Contains(o.SubjectID)));
                //}
                //if (!string.IsNullOrEmpty(GradeID))
                //{
                //    filter.Add(Builders<ExamQuestionArchiveEntity>.Filter.Where(o => o.GradeID == GradeID));
                //}

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
                            //GradeName = _gradeService.GetItemByID(o.GradeID)?.Name,
                            //SubjectName = _subjectService.GetItemByID(o.SubjectID)?.Name,
                            TotalQuestion = (int)_lessonPartExtensionService.CreateQuery().Find(x => x.ExamQuestionArchiveID == o.ID).CountDocuments(),
                            MainSubjectName = String.IsNullOrEmpty(o.MainSubjectID) ? "" : _mainSubjectService.GetItemByID(o.MainSubjectID).Name
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

                var filter = new List<FilterDefinition<ManageExamEntity>>();
                filter.Add(Builders<ManageExamEntity>.Filter.Where(o => o.Center.Contains(center.ID)));
                if (!String.IsNullOrEmpty(model.SearchText))
                {
                    filter.Add(Builders<ManageExamEntity>.Filter.Text("\"" + model.SearchText + "\""));
                }

                List<ManageExamEntity> listData = new List<ManageExamEntity>();
                if(_teacherHelper.HasRole(UserID,center.ID, "head-teacher"))
                {
                    
                    //var data = _manageExamService.GetItemsByTeacherAndCenter("",center.ID);
                    var data = _manageExamService.Collection.Find(Builders<ManageExamEntity>.Filter.And(filter));
                    var newData = (from d in data.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).SortByDescending(x=>x.Created).ToList()
                                   let user = _teacherService.GetItemByID(d.CreateUser)
                                  let listExam = _lessonExamService.GetItemsByManageExamID(d.ID)
                                  let totalExam = listExam.Count()
                                  select new ManageExamViewModel(d)
                                  {
                                      UserName = user == null ? "" : user.FullName,
                                      TotalExam = totalExam,
                                      ListExam = listExam,
                                      ClassName = d.ListClassID.Count() > 0 ? string.Join("; ", _classService.GetMultipleClassName(d.ListClassID)) : ""
                                  }).ToList();
                    listData.AddRange(newData);
                }
                else
                {
                    filter.Add(Builders<ManageExamEntity>.Filter.Where(o => o.CreateUser.Contains(UserID)));
                    //var data = _manageExamService.GetItemsByTeacherAndCenter(UserID,center.ID);
                    var data = _manageExamService.Collection.Find(Builders<ManageExamEntity>.Filter.And(filter)); ;
                    var newData = (from d in data.Skip(model.PageIndex * model.PageSize).Limit(model.PageSize).SortByDescending(x => x.Created).ToList()
                                   let user = _teacherService.GetItemByID(d.CreateUser)
                                   let listExam = _lessonExamService.GetItemsByManageExamID(d.ID)
                                   let totalExam = listExam.Count()
                                   select new ManageExamViewModel(d)
                                   {
                                       UserName = user == null ? "" : user.FullName,
                                       TotalExam = totalExam,
                                       ListExam = listExam,
                                       ClassName = d.ListClassID.Count() > 0 ? string.Join("; ", _classService.GetMultipleClassName(d.ListClassID)) : ""
                                   }).ToList();
                    listData.AddRange(newData);
                }
                model.TotalRecord = listData.Count();
                //var listClass = GetClasses(center.ID,tc.ID);
                return Json(new Dictionary<String, Object> 
                {
                    {"Status",true },
                    {"Data", listData },
                    //{"Classes",listClass },
                    {"Msg","" },
                     { "Model", model },
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
                        MainSubjectName = String.IsNullOrEmpty(item.MainSubjectID) ? "" : _mainSubjectService.GetItemByID(item.MainSubjectID).Name
                        //GradeName = _gradeService.GetItemByID(item.GradeID)?.Name,
                        //SubjectName = _subjectService.GetItemByID(item.SubjectID)?.Name
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
                        MainSubjectName = String.IsNullOrEmpty(oldItem.MainSubjectID) ? "" : _mainSubjectService.GetItemByID(oldItem.MainSubjectID).Name
                        //GradeName = _gradeService.GetItemByID(oldItem.GradeID)?.Name,
                        //SubjectName = _subjectService.GetItemByID(oldItem.SubjectID)?.Name
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
        public JsonResult CreateOrUpdateLessonPart(String basis, List<LessonPartExtensionEntity> lessonPartExtentsions, String ID, String GradeID,List<TagsEntity> tags)
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
                        TypePart = lessonPartExtension.TypePart,
                        Tags = tags.Count() == 0 ? new List<string>() : tags.Select(x=>x.ID).ToList()
                    };

                    //if(!String.IsNullOrEmpty(lessonPartExtension.Tags))
                    //{
                    //    var listTags = lessonPartExtension.Tags.Split(';');
                    //    var tag = "";
                    //    foreach(var t in listTags)
                    //    {
                    //        var codeT= t.ConvertUnicodeToCode("-", true);
                    //        Int32 pos = 0;
                    //        while (_tagsService.GetItemByCode(codeT) != null)
                    //        {
                    //            pos++;
                    //            codeT += ("-" + pos);
                    //        }
                    //        var newTag = new TagsEntity
                    //        {
                    //            Name = t,
                    //            Code = codeT,
                    //            ExamQuestionArchiveID = ID,
                    //            CenterCode = center.Code,
                    //            CreateUser = UserID
                    //        };
                    //        _tagsService.Save(newTag);
                    //        tag += $"{codeT}; ";
                    //    }
                    //    lessonpart.Tags = tag;
                    //}    

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
                                   //let tagName = _tagsService.GetNamesByCodes(lp.Tags)
                                   select new LessonPartExtensionViewModel(lp)
                                   {
                                       //TagsName = tagName
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
                                    let listtags = _tagsService.GetList(lp.Tags)
                                    select new LessonPartExtensionViewModel(lp)
                                    {
                                        ListTags = listtags
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

        public JsonResult ChangeTypePart(String basis,String ID,Int32 NewTypePart)
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

                lessonpart.TypePart = NewTypePart;
                lessonpart.Updated = DateTime.Now;
                _lessonPartExtensionService.Save(lessonpart);
                return Json(new Dictionary<String, Object>
                {
                    {"ID",lessonpart.ID },
                    {"NewType",lessonpart.TypePart }
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

        private List<ClassEntity> GetClasses(String centerID,String UserID)
        {
            try
            {
                var isHeadTeacher = _teacherHelper.HasRole(UserID, centerID, "head-teacher");
                var data = new List<ClassEntity>();
                if (isHeadTeacher)
                {
                    var listClass = _classService.CreateQuery().Find(x => x.Center.Equals(centerID) && x.IsActive && x.EndDate > DateTime.UtcNow && x.ClassMechanism != CLASS_MECHANISM.PERSONAL);
                    if(listClass.CountDocuments() > 0)
                    {
                        data.AddRange(listClass.ToList());
                    }
                    else
                    {
                        data.Add(new ClassEntity { Name = "Không có lớp nào" });
                    }
                }
                else
                {
                    var listClass = _classService.GetTeacherClassList(UserID);
                    if (listClass.Count() > 0)
                    {
                        data.AddRange(listClass.ToList());
                    }
                    else
                    {
                        data.Add(new ClassEntity { Name = "Không có lớp nào" });
                    }
                }
                return data;
            }
            catch(Exception ex)
            {
                var newclass = new ClassEntity
                {
                    Name = ex.Message
                };
                return new List<ClassEntity> { newclass };
            }
        }
        #endregion

        #region CreateExam
        public async Task<JsonResult> CreateOrUpdateExam(
            DefaultModel model, 
            String basis, 
            ExamProcessViewModel item,
            List<MatrixExamViewModel> matrixExams, 
            Boolean isNew)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                if (String.IsNullOrEmpty(UserID) || _teacherService.GetItemByID(UserID) == null)
                {
                    return Json("Tai khoan khong ton tai.");
                }

                var centerID = _centerService.GetItemByCode(basis).ID;
                MatrixExamEntity matrixExam = new MatrixExamEntity();

                List<String> Tags = new List<string>();
                if (String.IsNullOrEmpty(item.MatrixID))
                {
                    //Lưu format đề
                   matrixExam = CreateOrUpdateMatrix(item, matrixExams, UserID, centerID, out Tags);
                }
                else
                {
                    matrixExam = _matrixExamService.GetItemByID(item.MatrixID);
                }

                //if (item.Template == EXAM_TYPE.ISLECTURE) // luyen tap thi khong tao de truoc
                //{
                //    return Json("Chức năng đang chờ");
                //}
                //else //kiem tra thi tao de truoc
                //{
                if (isNew)
                {
                    List<String> listLessonExamIDs = new List<string>();
                    List<LessonExamEntity> lessonExams = new List<LessonExamEntity>();
                    //List<Int32> listIndex = new List<Int32>();
                    //var rd = new Random();
                    //var index = rd.Next(100, 999);

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

                    var listIndexs = RandomIndex(item.TotalExam, item.TotalExam);

                    //tạo mã đề, gán cho từng kì thi
                    for (Int32 i = 0; i < listIndexs.Count(); i++)
                    {
                        
                        var codeExam = listIndexs.ElementAtOrDefault(i);
                        //listIndex.Add(index);

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
                            CodeExam = codeExam.ToString(),
                            ManageExamID = manageexam.ID,
                            StartDate = manageexam.StartDate,
                            EndDate = manageexam.EndDate
                        };
                        _lessonExamService.Save(lessonExam);
                        //lessonExams.Add(lessonExam);
                        listLessonExamIDs.Add(lessonExam.ID);
                        var str = RenderExam(lessonExam, matrixExam, item.ExamQuestionArchiveID, UserID, Tags).Result;
                    }

                    var user = _teacherService.GetItemByID(UserID);
                    var listExam = _lessonExamService.GetItemsByManageExamID(manageexam.ID);
                    var totalExam = listExam.Count();
                    var dataresponse = new ManageExamViewModel(manageexam)
                    {
                        UserName = user == null ? "" : user.FullName,
                        TotalExam = totalExam,
                        ListExam = totalExam == 0 ? new List<LessonExamEntity>() : listExam,
                        ClassName = ""
                    };
                    return Json(new Dictionary<String, Object> {
                        {"Status",true },
                        {"Data", dataresponse},
                        {"Msg","Tạo đề thành công" }
                    });
                }
                else
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Data", new ManageExamEntity()},
                        {"Msg","is new = false" }
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public JsonResult RemoveExam(String ID)
        {
            try
            {
                var manageExam = _manageExamService.GetItemByID(ID);
                if(manageExam == null)
                {
                        return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Data",null },
                        {"Msg","Không tìm thấy thông tin kì thi." }
                    });
                }

                var lessonExams= _lessonExamService.GetItemsByManageExamID(ID);
                var lessonExamIDs = lessonExams.Select(x => x.ID).ToList();
                var lesson = _lessonService.CreateQuery().Find(x => x.LessonExamID.Intersect(lessonExamIDs).Any()).Project(x => x.ID).ToList();
                
                foreach(var l in lesson)
                {
                    _lessonService.Remove(l);
                }



                _manageExamService.Remove(ID);
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Data",null },
                    {"Msg","" }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Data",null },
                    {"Msg",ex.Message }
                });
            }
        }

        private MatrixExamEntity CreateOrUpdateMatrix(
            ExamProcessViewModel item, 
            List<MatrixExamViewModel> matrixExams, 
            string UserID, 
            string centerID, 
            out List<String> Tags)
        {
            var matrixExam = new MatrixExamEntity
            {
                Name = matrixExams.FirstOrDefault().Name,
                ExamQuestionArchiveID = item.ExamQuestionArchiveID,
                Created = DateTime.UtcNow,
                CreateUser = UserID,
                Center = centerID,
            };

            List<String> _Tags = new List<string>();

            for (var i = 0; i < matrixExams.Count; i++)
            {
                var f = matrixExams.ElementAtOrDefault(i);
                f.Know.Total = f.Know.Theory + f.Know.Exercise;
                f.Understanding.Total = f.Understanding.Theory + f.Understanding.Exercise;
                f.Manipulate.Total = f.Manipulate.Theory + f.Manipulate.Exercise;
                f.ManipulateHighly.Total = f.ManipulateHighly.Theory + f.ManipulateHighly.Exercise;
                var total = f.Know.Total + f.Understanding.Total + f.Manipulate.Total + f.ManipulateHighly.Total;
                var detail = new DetailMatrixExam
                {
                    Level = f.Level,
                    Order = i,
                    //Tags = f.Tags,
                    Know = f.Know,
                    Understanding = f.Understanding,
                    Manipulate = f.Manipulate,
                    ManipulateHighly = f.ManipulateHighly,
                    Total = total
                };
                matrixExam.DetailFormat.Add(detail);
                _Tags.AddRange(f.Tags);
            }

            Tags = _Tags;
            _matrixExamService.Save(matrixExam);
            return matrixExam;
        }

        private async Task<String> RenderExam(LessonExamEntity lessonExam, MatrixExamEntity matrixExam, String ID,String UserID,List<String> Tags)
        {
            //lay danh sach cac lessonpart trong ngan hang cau hoi
            //var lessonParts = _lessonPartExtensionService.GetItemsByExamQuestionArchiveID(ID);
            var lessonParts = _lessonPartExtensionService.CreateQuery().Find(x=>x.ExamQuestionArchiveID == ID && Tags.Intersect(x.Tags).Any()).ToEnumerable();
            if (lessonParts.Count() == 0)
            {
                return "";
            }

            var lessonpartIDs = lessonParts.Select(x => x.ID).ToList();
            var listQuestions = _lessonPartQuestionExtensionServie.CreateQuery().Find(x => lessonpartIDs.Contains(x.ParentID));
            var listQuestionIDs = listQuestions.Project(x => x.ID).ToList();
            var listAns = _lessonPartAnswerExtensionService.CreateQuery().Find(x => listQuestionIDs.Contains(x.ParentID));

            var newListParts = new List<LessonPartExtensionEntity>();
            var newListQuestion = new List<LessonPartQuestionExtensionEntity>();
            var newListAns = new List<LessonPartAnswerExtensionEntity>();
            foreach (var item in matrixExam.DetailFormat)
            {
                var _listPartKnow = lessonParts.Where(x => x.LevelPart == LEVELPART.KNOW); // 
                //var _listPartKnow = lessonParts.Where(x => x.LevelPart == LEVELPART.KNOW && item.Tags.Contains(x.Tags)).Take(item.Total); // 
                {
                    var partKnowTheory = _listPartKnow.Where(x => x.TypePart == TYPE_PART.THEORY);
                    var partKnowExercise = _listPartKnow.Where(x => x.TypePart == TYPE_PART.EXERCISE);

                    //lấy câu lý thuyết
                    newListParts.AddRange(GetParts(partKnowTheory.ToList(),item.Know.Theory));
                    //lấy câu bài tập
                    newListParts.AddRange(GetParts(partKnowExercise.ToList(),item.Know.Exercise));
                }
                var _listPartUnderstanding = lessonParts.Where(x => x.LevelPart == LEVELPART.UNDERSTANDING );
                {
                    var partUnderstandingTheory = _listPartKnow.Where(x => x.TypePart == TYPE_PART.THEORY);
                    var partUnderstandingExercise = _listPartKnow.Where(x => x.TypePart == TYPE_PART.EXERCISE);

                    //lấy câu lý thuyết
                    newListParts.AddRange(GetParts(partUnderstandingTheory.ToList(), item.Understanding.Theory));
                    //lấy câu bài tập
                    newListParts.AddRange(GetParts(partUnderstandingExercise.ToList(),item.Understanding.Exercise));
                }
                var _listPartManipulate = lessonParts.Where(x => x.LevelPart == LEVELPART.MANIPULATE );
                {
                    var partManipulateTheory = _listPartKnow.Where(x => x.TypePart == TYPE_PART.THEORY);
                    var partManipulateExercise = _listPartKnow.Where(x => x.TypePart == TYPE_PART.EXERCISE);

                    //lấy câu lý thuyết
                    newListParts.AddRange(GetParts(partManipulateTheory.ToList(), item.Manipulate.Theory));
                    //lấy câu bài tập
                    newListParts.AddRange(GetParts(partManipulateExercise.ToList(), item.Manipulate.Exercise));
                }
                var _listPartManipulateHighly = lessonParts.Where(x => x.LevelPart == LEVELPART.MANIPULATEHIGHLY );
                {
                    var partManipulateHighlyTheory = _listPartKnow.Where(x => x.TypePart == TYPE_PART.THEORY);
                    var partManipulateHighlyExercise = _listPartKnow.Where(x => x.TypePart == TYPE_PART.EXERCISE);

                    //lấy câu lý thuyết
                    newListParts.AddRange(GetParts(partManipulateHighlyTheory.ToList(), item.ManipulateHighly.Theory));
                    //lấy câu bài tập
                    newListParts.AddRange(GetParts(partManipulateHighlyExercise.ToList(), item.ManipulateHighly.Exercise));
                }
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
                    Media = part.Media == null ? null : new Media { Created = part.Media.Created, Extension = part.Media.Extension, Name = part.Media.Name, OriginalName = part.Media.OriginalName, Path = part.Media.Path, Size = part.Media.Size },
                    LessonExamID = lessonExam.ID
                };

                _cloneLessonPartExtensionService.Save(clonelessonpart);
                SetQuestion(UserID, null, newListQuestion, newListAns, part, clonelessonpart);
            }

            return "";
        }

        private List<LessonPartExtensionEntity> GetParts( 
            //DetailMatrixExam item,
            List<LessonPartExtensionEntity> listParts,
            Int32 TotalPart
            )
        {
            List<LessonPartExtensionEntity> newListParts = new List<LessonPartExtensionEntity>();

            //lay cau li thuye
            if (TotalPart > 0 && TotalPart == 1)
            {
                var index = new Random().Next(0, listParts.Count());
                var part = listParts.ElementAtOrDefault(index);
                newListParts.Add(part);
            }
            else if (TotalPart > 1)
            {
                var indexs = RandomIndex(TotalPart, listParts.Count(), 0);
                var part = (from i in indexs
                            let p = listParts.ElementAtOrDefault(i)
                            where p != null
                            select p).ToList();
                newListParts.AddRange(part);
            }
            return newListParts;
        }

        public JsonResult UpdateExam(String ID)
        {
            try
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg","" },
                    {"Data",null }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg",ex.Message },
                    {"Data",null }
                });
            }
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

        private void SetQuestion(
            string UserID, 
            LessonExamEntity lesson, 
            List<LessonPartQuestionExtensionEntity> newListQuestion, 
            List<LessonPartAnswerExtensionEntity> newListAns, 
            LessonPartExtensionEntity part, 
            CloneLessonPartExtensionEntity clonelessonpart)
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
                    LessonID = lesson == null ? "" : lesson.ID,
                    LessonExamID = clonelessonpart.LessonExamID
                };

                _cloneLessonPartQuestionExtensionService.Save(clonelessonpartquestion);
                if (part.Type == "QUIZ3")
                {
                    String a = "";
                }
                SetAnswer(UserID, lesson, newListAns, quiz, clonelessonpartquestion);
            }
        }

        private async Task<List<LessonPartQuestionExtensionEntity>> GetQuestion(
            List<LessonPartExtensionEntity> lessonPartExtensions, 
            List<LessonPartQuestionExtensionEntity> lessonPartQuestions)
        {
            List<LessonPartQuestionExtensionEntity> questions = new List<LessonPartQuestionExtensionEntity>();
            foreach (var item in lessonPartExtensions)
            {
                if (item != null)
                {
                    var question = lessonPartQuestions.Where(x => x.ParentID == item.ID).ToList();
                    if (question == null) continue;
                    questions.AddRange(question);
                }
            }
            return questions;
        }

        private void SetAnswer(
            string UserID, 
            LessonExamEntity lesson, 
            List<LessonPartAnswerExtensionEntity> newListAns, 
            LessonPartQuestionExtensionEntity quiz, 
            CloneLessonPartQuestionExtensionEntity clonelessonpartquestion)
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

        private async Task<List<LessonPartAnswerExtensionEntity>> GetAnswer(
            List<LessonPartQuestionExtensionEntity> lessonPartQuestions, 
            List<LessonPartAnswerExtensionEntity> lessonPartAnswers)
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

        public async Task<JsonResult> SetupExam(String basis, ManageExamEntity item,List<String> ListClassID)
        {
            var UserID = User.Claims.GetClaimByType("UserID").Value;
            try
            {
                var manageExam = _manageExamService.GetItemByID(item.ID);
                if(manageExam == null)
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Msg","Không tìm thấy kì thi." },
                        {"Data",null }
                    });
                }

                List<String> listClassIDs = new List<string>();
                if(item.ListClassID.Count() > ListClassID.Count) //xoa lop
                {
                    manageExam.ListClassID = new List<string>();
                    foreach(var c in ListClassID)
                    {
                        if (item.ListClassID.Any(x => x.Equals(c)))
                        {
                            listClassIDs.Add(c);
                        }
                    }
                }
                else if(item.ListClassID.Count() < ListClassID.Count) //them lop
                {
                    foreach (var c in ListClassID)
                    {
                        if (!item.ListClassID.Any(x => x.Equals(c)))
                        {
                            listClassIDs.Add(c);
                        }
                    }
                }

                var csbjs = _classSubjectService.GetClassSubjectExamByClassIDs(ListClassID);
                if(csbjs.Count() == 0)
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Msg","Không tìm thấy thông tin lớp." },
                        {"Data",null }
                    });
                }

                var lessonExams = _lessonExamService.GetItemsByManageExamID(item.ID);
                foreach(var lessonExam in lessonExams)
                {
                    lessonExam.StartDate = item.StartDate;
                    lessonExam.EndDate = item.EndDate;
                    _lessonExamService.Save(lessonExam);
                }
                var lessonExamIDs = lessonExams.Select(x => x.ID).ToList();
                if(lessonExams.Count() == 0)
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Msg","Không tìm thấy thông tin đề thi." },
                        {"Data",null }
                    });
                }

                var lessonParts = _cloneLessonPartExtensionService.GetItemsByLessonExamID(lessonExamIDs);
                var lessonPartsQuiz = _cloneLessonPartQuestionExtensionService.GetItemsByLessonExamID(lessonExamIDs);
                var lessonPartsQuizIDs = lessonPartsQuiz.Select(x => x.ID).ToList();
                var lessonPartsAns = _cloneLessonPartAnswerExtensionService.GetItemsByQuestionIDs(lessonPartsQuizIDs);

                foreach (var cs in csbjs)
                {
                    var lesson = new LessonEntity
                    {
                        TemplateType = 2,
                        Timer = item.Timer,
                        CreateUser = UserID,
                        Title = $"{manageExam.Name}",
                        Created = DateTime.Now,
                        Updated = DateTime.Now,
                        Limit = manageExam.Limtit,
                        Multiple = 1,
                        Etype = 0,
                        ClassID = cs.ClassID,
                        ClassSubjectID = cs.ID,
                        ChapterID = "0",
                        IsParentCourse = true,
                        LessonExamID = lessonExamIDs,
                        StartDate = item.StartDate,
                        EndDate = item.EndDate
                    };
                    _lessonService.Save(lesson);
                    //CopyLessonPart(lesson, lessonParts.ToList(), lessonPartsQuiz.ToList(), lessonPartsAns.ToList(), lessonExamIDs);
                    foreach (var lessonExam in lessonExams)
                    {
                    }
                }

                manageExam.ListClassID.AddRange(listClassIDs);
                manageExam.StartDate = item.StartDate;
                manageExam.EndDate = item.EndDate;
                _manageExamService.Save(manageExam);

                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Msg","" },
                    {"Data",null }
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

        private async Task<CloneLessonPartEntity> CopyLessonPart(
            LessonEntity lesson
            , List<CloneLessonPartExtensionEntity> cloneLessonPartExtensions
            , List<CloneLessonPartQuestionExtensionEntity> cloneLessonPartQuestionExtensions
            , List<CloneLessonPartAnswerExtensionEntity> cloneLessonPartAnswerExtensions
            , List<String> lessonExamIDs
            )
        {
            try
            {
                foreach (var lessonExamID in lessonExamIDs)
                {
                    var parts = cloneLessonPartExtensions.Where(x => x.LessonExamID == lessonExamID);
                    var questions = cloneLessonPartQuestionExtensions.Where(x => x.LessonExamID == lessonExamID);
                    var questionIDs = questions.Select(x => x.ID);
                    var ans = cloneLessonPartAnswerExtensions.Where(x => questionIDs.Contains(x.ParentID));

                    foreach (var part in parts)
                    {
                        var newPart = new CloneLessonPartEntity
                        {
                            OriginID = part.ID,
                            ParentID = lesson.ID,
                            Title = part.Title,
                            Timer = part.Timer,
                            Description = part.Description,
                            Type = part.Type,
                            Point = part.Point,
                            Created = DateTime.UtcNow,
                            Updated = DateTime.UtcNow,
                            Order = part.Order,
                            Media = part.Media,
                            CourseID = part.CourseID,
                            TeacherID = lesson.CreateUser,
                            ClassID = lesson.ClassID,
                            ClassSubjectID = lesson.ClassSubjectID
                        };

                        _cloneLessonPartService.Save(newPart);
                        var listQuiz = questions.Where(x => x.ParentID == part.ID).ToList();
                        var listAns = ans.Where(x => listQuiz.Select(y => y.ID).ToList().Contains(x.ParentID)).ToList();
                        CopyQuiz(newPart.ID, listQuiz, listAns, lesson);
                    }
                }
                return new CloneLessonPartEntity();
            }
            catch (Exception ex)
            {
                return new CloneLessonPartEntity { Title = ex.Message };
            }
        }

        private async Task CopyQuiz(
            String partID
            , List<CloneLessonPartQuestionExtensionEntity> cloneLessonPartQuestionExtensions
            , List<CloneLessonPartAnswerExtensionEntity> cloneLessonPartAnswerExtensions
            , LessonEntity lesson
        )
        {
            foreach(var q in cloneLessonPartQuestionExtensions)
            {
                var newQ = new CloneLessonPartQuestionEntity
                {
                    OriginID = q.ID,
                    Content = q.Content,
                    CreateUser = lesson.CreateUser,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Point = q.Point,
                    Order = q.Order,
                    Description = q.Description,
                    Media = q.Media,
                    ParentID = partID,
                    CourseID = q.CourseID,
                    TeacherID = lesson.CreateUser,
                    ClassID = lesson.ClassID,
                    ClassSubjectID = lesson.ClassSubjectID,
                    LessonID = lesson.ID
                };

                _cloneLessonPartQuestionService.Save(newQ);
                var listAns = cloneLessonPartAnswerExtensions.Where(x => x.ParentID == q.ID).ToList();
                CopyAns(newQ.ID, listAns,lesson);
            }
        }

        private async Task CopyAns(
            String quizID
            , List<CloneLessonPartAnswerExtensionEntity> cloneLessonPartAnswerExtensions
            , LessonEntity lesson
        )
        {
            foreach(var a in cloneLessonPartAnswerExtensions)
            {
                var newA = new CloneLessonPartAnswerEntity
                {
                    OriginID = a.ID,
                    ParentID = quizID,
                    Content = a.Content,
                    IsCorrect = a.IsCorrect,
                    CreateUser = lesson.CreateUser,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow,
                    Media = a.Media,
                    Order = a.Order,
                    CourseID = a.CourseID,
                    TeacherID = lesson.CreateUser,
                    ClassID = lesson.ClassID,
                    ClassSubjectID = lesson.ClassSubjectID
                };
                _cloneLessonPartAnswerService.Save(newA);
            }
        }

        #endregion

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

        public List<Int32> RandomIndex(Int32 TotalIndex, Int32 max,Int32 min = 0)
        {
            var rd = new Random();
            List<Int32> listIndex = new List<int>();
            do
            {
                Int32 index = rd.Next(min, max);
                if (!listIndex.Contains(index))
                {
                    listIndex.Add(index);
                }
            }
            while (listIndex.Count() != TotalIndex);
            return listIndex;
        }

        //public JsonResult ShowDetailExam(String ID)
        //{
        //    try
        //    {
        //            var lesson = _lessonService.CreateQuery().Find(x => x.LessonExamID == ID).FirstOrDefault();
        //            return Json(new Dictionary<String, object> {
        //                {"ClassSubjectID",lesson.ClassSubjectID },
        //                {"LessonID",lesson.ID }
        //            });
        //    }
        //    catch(Exception ex)
        //    {
        //        return Json(ex.Message);
        //    }
        //}


        #region Tags
        public JsonResult SearchTags(String Term,String basis)
        {
            try
            {
                var filter = new List<FilterDefinition<TagsEntity>>();
                var center = _centerService.GetItemByCode(basis);
                if (center == null)
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Data", null },
                        {"Msg","Cơ sở không đúng" }
                    });
                }

                //filter.Add(Builders<TagsEntity>.Filter.Where(o => o.CenterCode.Contains(center.Code)));

                if (!string.IsNullOrEmpty(Term))
                    filter.Add(Builders<TagsEntity>.Filter.Text("\"" + Term + "\""));

                var data = _tagsService.CreateQuery().Find(Builders<TagsEntity>.Filter.And(filter)).Limit(100).ToList();

                //if (data.Count() == 0)
                //    data = _tagsService.GetAll().ToList();
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Data", data },
                    {"Msg","" }
                });
            }
            catch(Exception ex)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Data", null },
                    {"Msg",ex.Message }
                });
            }
        }

        public JsonResult UpdateTags(String basis,string lessonpartID,List<TagsEntity> listTags,Boolean isRemove = false)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var center = _centerService.GetItemByCode(basis);
                var lessonpart = _lessonPartExtensionService.GetItemByID(lessonpartID);
                var resData = new List<TagsEntity>();
                if (!isRemove)
                {
                    foreach (var tags in listTags)
                    {
                        if (String.IsNullOrEmpty(tags.ID))
                        {
                            var codeT = tags.Name.ConvertUnicodeToCode("-", true);
                            Int32 pos = 0;
                            while (_tagsService.GetItemByCode(codeT) != null)
                            {
                                pos++;
                                codeT += ("-" + pos);
                            }
                            var newTag = new TagsEntity
                            {
                                Name = tags.Name,
                                Code = codeT,
                                ExamQuestionArchiveID = tags.ExamQuestionArchiveID,
                                CenterCode = center.Code,
                                CreateUser = UserID,
                                ParentIDs = tags.ParentIDs == null ? new List<string>() : tags.ParentIDs
                            };
                            _tagsService.Save(newTag);
                            resData.Add(newTag);
                            if (lessonpart != null)
                            {
                                if (tags != null)
                                {
                                    lessonpart.Tags.Add(newTag.ID);
                                }
                                else
                                {
                                    lessonpart.Tags = new List<string>();
                                    lessonpart.Tags.Add(newTag.ID);
                                }
                            }
                        }
                        else
                        {
                            if (lessonpart != null)
                            {
                                if (!lessonpart.Tags.Contains(tags.ID))
                                {
                                    lessonpart.Tags.Add(tags.ID);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (listTags.Count() > 0)
                    {
                        lessonpart.Tags.Remove(listTags.FirstOrDefault().ID);
                    }
                }

                if (lessonpart != null)
                {
                    _lessonPartExtensionService.Save(lessonpart);
                }
                return Json(new Dictionary<String, Object> {
                    {"Status",true },
                    {"Data",resData},
                    {"Msg","" }
                });
            }
            catch(Exception e)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Data",null },
                    {"Msg",e.Message }
                });
            }
        }
        #endregion
        public IActionResult Detail(String LessonExamID)
        {
            ViewBag.LessonExam = _lessonExamService.GetItemByID(LessonExamID);
            ViewBag.Class = new ClassEntity();
            ViewBag.Subject = new ClassSubjectEntity();
            return View("Detail");
        }

        //get list main subject
        public JsonResult GetMainSubjects(String basis)
        {
            try
            {
                var UserID = User.Claims.GetClaimByType("UserID").Value;
                var tc = _teacherService.GetItemByID(UserID);
                if(tc == null)
                {
                    return Json(new Dictionary<String, Object> {
                        {"Status",false },
                        {"Data",null },
                        {"Msg","Thông tin giáo viên không đúng" }
                    });
                }

                var sbjs = tc.Subjects;
                var mainsubjects = _mainSubjectService.CreateQuery().Find(x => sbjs.Contains(x.ID)).ToList();
                return Json(new Dictionary<String, Object> {
                        {"Status",true },
                        {"Data",mainsubjects },
                        {"Msg","" }
                    });
            }
            catch(Exception e)
            {
                return Json(new Dictionary<String, Object> {
                    {"Status",false },
                    {"Data",null },
                    {"Msg",e.Message }
                });
            }
        }
    }
}
