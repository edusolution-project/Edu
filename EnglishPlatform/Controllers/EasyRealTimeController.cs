using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseCustomerEntity.Database;
using BaseCustomerMVC.Controllers.Student;
using BaseCustomerMVC.Globals;
using BaseEasyRealTime.Entities;
using FileManagerCore.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace EnglishPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EasyRealTimeController : ControllerBase
    {
        #region const
        const string Student = "student";
        const string Teacher = "teacher";
        const string Admin = "admin";
        #endregion
        #region CurrentUser
        private string _userID { get; set; }
        private string _typeUser { get; set; }
        public string _name { get; set; }
        public string _email { get; set; }
        #endregion
        private readonly CommentService _commentService;
        private readonly NotificationService _notificationService;
        private readonly IRoxyFilemanHandler _roxyFilemanHandler;
        private readonly StudentService _studentService;
        private readonly TeacherService _teacherService;
        private readonly SubjectService _subjectService;
        private readonly ClassService _classService;
        private readonly ClassStudentService _classStudentService;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly GroupService _groupService;
        private readonly MessageService _messageService;
        private readonly NewFeedService _newFeedService;
        private readonly IHubContext<ChatHub> _hubContent;

        public EasyRealTimeController(
            CommentService commentService,
            NotificationService notificationService,
            StudentService studentService,
            TeacherService teacherService,
            SubjectService subjectService,
            ClassService classService,
            ClassStudentService classStudentService,
            LessonScheduleService lessonScheduleService,
            GroupService groupService,
            MessageService messageService,
            NewFeedService newFeedService,
            IHubContext<ChatHub> hubContent,
            IRoxyFilemanHandler roxyFilemanHandler
        )
        {
            _studentService = studentService;
            _teacherService = teacherService;
            _subjectService = subjectService;
            _classService = classService;
            _classStudentService = classStudentService;
            _lessonScheduleService = lessonScheduleService;
            _groupService = groupService;
            _messageService = messageService;
            _hubContent = hubContent;
            _roxyFilemanHandler = roxyFilemanHandler;
            _notificationService = notificationService;
            _commentService = commentService;
            _newFeedService = newFeedService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetClassStatus()
        {
            try
            {
                if (IsAuthenticated())
                {
                    Dictionary<string, bool> req = new Dictionary<string, bool>();
                    var listClass = !_typeUser.Contains(Teacher) ? _classStudentService.GetStudentClasses(_userID) : null;
                    var realClass = listClass != null
                        ? _classService.CreateQuery().Find(o => listClass.Contains(o.ID))?.ToList()
                        : _classService.CreateQuery().Find(o => o.TeacherID == _userID)?.ToList();
                    for (int i = 0; realClass != null && i < realClass.Count; i++)
                    {
                        var item = realClass[i];
                        var isActive = item.StartDate <= DateTime.Now.ToUniversalTime() && item.EndDate >= DateTime.Now.ToUniversalTime();
                        if(isActive) req.Add(item.ID, isActive);
                    }
                    return Success(req);
                }
                return NotFoundData();
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }

        #region group
        /// <summary>
        /// laays danh sach group can
        /// </summary>
        /// <param name="searchText"> ten lop </param>
        /// <returns>
        /// Code = 200 , Message = "Success", Data = new { Group = groupList, Student = listMembers, Teacher = listMemberTeacher}
        /// Code = 404 , Message = "No Data Found" , DAta = null
        /// </returns>
        [HttpGet]
        public JsonResult GetGroupList()
        {
            try
            {
                if (IsAuthenticated())
                {
                    var listClass = !_typeUser.Contains(Teacher) ? _classStudentService.GetStudentClasses(_userID) : null;
                    var realClass = listClass != null
                        ? _classService.CreateQuery().Find(o => listClass.Contains(o.ID))?.ToList()
                        : _classService.CreateQuery().Find(o => o.TeacherID == _userID)?.ToList();
                    var listMembers = new HashSet<MemberGroupInfo>();
                    var listMemberTeacher = new HashSet<MemberGroupInfo>();
                    if (realClass != null)
                    {
                        // danh sach lop
                        var listClassID = realClass.Select(o => o.ID).ToList();
                        var liststudentClass = _classStudentService.CreateQuery().Find(o => listClassID.Contains(o.ClassID))?.ToList();
                        var listStudent = liststudentClass?.Select(o => o.StudentID);
                        var listTeacher = realClass.Select(o => o.TeacherID).Distinct();

                        if (listStudent != null)
                        {
                            listMembers = _studentService.CreateQuery().Find(o => listStudent.Contains(o.ID))?.ToList()?.Select(x => new MemberGroupInfo(x.ID, x.Email, x.FullName, false))?.ToHashSet();
                        }
                        if (listTeacher != null)
                        {
                            listMemberTeacher = _teacherService.CreateQuery().Find(o => listTeacher.Contains(o.ID))?.ToList()?.Select(x => new MemberGroupInfo(x.ID, x.Email, x.FullName, true))?.ToHashSet();
                        }
                        int countClassID = listClassID.Count;
                        // danh sach nhom bao gom 
                        var groupList = _groupService.CreateQuery().Find(o => o.IsPrivateChat == false && (listClassID.Contains(o.Name) || listClassID.Contains(o.ParentID)))?.ToList();
                        // danh sach goc
                        var listGroupName = groupList != null ? groupList.Where(o => o.ParentID == null || o.ParentID == "" || o.ParentID == "null")?.Select(o => o.Name) : new List<string>();
                        if (listGroupName == null || listGroupName?.Count() < countClassID)
                        {
                            for (int i = 0; i < countClassID; i++)
                            {
                                var itemClass = realClass[i];
                                if (listGroupName == null || !listGroupName.Contains(itemClass.ID))
                                {
                                    var listMembersStudentID = _classStudentService.GetClassStudents(itemClass.ID)?.Select(o => o.StudentID)?.ToList();
                                    var members = _studentService.CreateQuery().Find(o => listStudent.Contains(o.ID))?.ToList()?.Select(x => new MemberGroupInfo(x.ID, x.Email, x.FullName, false))?.ToHashSet();
                                    var teacher = _teacherService.GetItemByID(itemClass.TeacherID);
                                    if (members == null)
                                    {
                                        members = new HashSet<MemberGroupInfo>() {
                                            new MemberGroupInfo(teacher.ID,teacher.Email, teacher.FullName, true)
                                        };
                                    }
                                    else
                                    {
                                        for (int x = 0; x < listMemberTeacher.Count; x++)
                                        {
                                            members.Add(new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, true));
                                        }

                                    }
                                    var newGroup = new GroupEntity()
                                    {
                                        IsPrivateChat = false,
                                        DisplayName = itemClass.Name,
                                        Name = itemClass.ID,
                                        Status = true,
                                        Created = DateTime.Now,
                                        CreateUser = itemClass.TeacherID,
                                        MasterGroup = new HashSet<MemberGroupInfo>() { new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, true) },
                                        Members = members
                                    };
                                    _groupService.CreateOrUpdate(newGroup);
                                }
                            }
                        }
                        groupList = _groupService.CreateQuery().Find(o => o.IsPrivateChat == false && listClassID.Contains(o.Name))?.ToList();
                        return Success(new { Group = groupList, Student = listMembers, Teacher = listMemberTeacher });
                    }
                }

                return NotFoundData();
            }
            catch (Exception ex)
            {
                _ = _groupService.RemoveAllAsync();
                return Error(ex);
            }
        }
        [HttpPut]
        public JsonResult UpdateGroup(GroupEntity group)
        {
            if (IsAuthenticated())
            {
                if (!string.IsNullOrEmpty(group.ID))
                {
                    var oldGroup = _groupService.GetItemByID(group.ID);
                    if(oldGroup != null)
                    {
                        _groupService.CreateOrUpdate(group);
                        return Success(group);
                    }
                }
            }

            return NotFoundData();
        }

        [HttpDelete]
        public JsonResult RemoveGroup(string groupName)
        {
            if (IsAuthenticated())
            {
                var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();
                if (group != null)
                {
                    if (group.MasterGroup.Contains(new MemberGroupInfo(_userID, _email, _name, _typeUser.Contains(Teacher))))
                    {
                        _groupService.Remove(group.ID);
                        return Success(group.ID);
                    }
                    else
                    {
                        return ResponseApi(405, "You are not the master group !!!");
                    }
                }
            }

            return NotFoundData();
        }

        [HttpPost]
        public JsonResult CreateGroup(string groupName, string groupParent, List<MemberGroupInfo> members)
        {
            if (IsAuthenticated())
            {
                if (members != null)
                {
                    var newGroupName = Guid.NewGuid().ToString();
                    bool IsPrivate = string.IsNullOrEmpty(groupParent); // user to user / group
                    var group = new GroupEntity()
                    {
                        Created = DateTime.Now,
                        Name = newGroupName,
                        CreateUser = _userID,
                        DisplayName = groupName,
                        Members = members.ToHashSet(),
                        IsPrivateChat = IsPrivate,
                        
                    };
                    // user to user
                    if (!IsPrivate)
                    {
                        group.ParentID = groupParent;
                        group.MasterGroup = new HashSet<MemberGroupInfo>() { new MemberGroupInfo(_userID,_email,_name, _typeUser.Contains(Teacher)) };
                    }
                    // taoj ra la sub cua lop
                    else
                    {
                        var master = members.Where(o => o.IsTeacher)?.ToHashSet();
                        if(master == null)
                        {
                            var _groupParent = _groupService.CreateQuery().Find(o => o.Name == groupParent)?.FirstOrDefault();

                            if (_groupParent == null) return ResponseApi(404, "Not Found group parent");

                            master = _groupParent.MasterGroup;
                        }

                        if(master == null) return ResponseApi(404,"Not Found group master");

                        group.MasterGroup = master;
                    }
                    _groupService.CreateOrUpdate(group);

                    return Success(group);
                }
            }

            return NotFoundData();
        }
        [HttpPost]
        public JsonResult AddMemberToGroup(string memberID,string groupName)
        {
            if (IsAuthenticated())
            {
                var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();
                if (group != null)
                {
                    var student = _studentService.GetItemByID(memberID);

                    if (student != null)
                    {
                        var newMember = new MemberGroupInfo(student.ID, student.Email, student.FullName, false);
                        if (group.Members.Contains(newMember))
                        {
                            group.Members.Add(newMember);
                            return Success(new { Member = newMember });
                        }
                    }
                    var teacher = _teacherService.GetItemByID(memberID);
                    if (teacher != null)
                    {
                        var newMember = new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, true);
                        if (group.Members.Contains(newMember))
                        {
                            group.Members.Add(newMember);
                            return Success(new { Member = newMember });
                        }
                    }
                }
            }

            return NotFoundData();
        }
        /// <summary>
        /// lay danh sach member trong group
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetMemberFromList(string groupName)
        {
            if (IsAuthenticated())
            {
                var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();
                if (group != null)
                {
                    return Success(new { group.Members });
                }
            }

            return NotFoundData();
        }
        #endregion
        #region Message
        [HttpGet]
        public JsonResult GetListMessage(string groupName, int state , DateTime startDate , DateTime endDate, bool IsUser, bool IsTeacher)
        {
            if (IsAuthenticated())
            {
                var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();
                if (IsUser)
                {
                    var sender = new MemberGroupInfo(_userID, _email, _name, _typeUser.Contains(Teacher));
                    if (IsTeacher)
                    {
                        var teacher = _teacherService.GetItemByID(groupName);
                        if(teacher != null)
                        {
                            var receiver = new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, IsTeacher);
                            group = _groupService.CreateNewGroup(sender, receiver);
                        }
                    }
                    else
                    {
                        var student = _studentService.GetItemByID(groupName);
                        if (student != null)
                        {
                            var receiver = new MemberGroupInfo(student.ID, student.Email, student.FullName, IsTeacher);
                            group = _groupService.CreateNewGroup(sender, receiver);
                        }
                    }
                }
                if (group != null) {

                    var message = state == 0 ? _messageService.GetMessageList(group.Name, startDate, endDate) : _messageService.GetNewFeedList(group.Name, startDate, endDate);
                    return Success(new { messages= message});
                }
            }

            return NotFoundData();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"> name group </param>
        /// <param name="reply">Code messsage rep</param>
        /// <param name="state"> 0 => chat , 1 => newfeed  </param>
        /// <param name="title"> null / string</param>
        /// <param name="content"> null/ string </param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Create([FromForm]string groupName,[FromForm] string reply, [FromForm] int state, [FromForm] string title,[FromForm] string content)
        {
            if (IsAuthenticated())
            {
                var sender = new MemberGroupInfo(_userID, _email, _name, _typeUser.Contains(Teacher));
                var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();

                if(group == null)
                {
                    
                    MemberGroupInfo receiver = null;
                    var teacher = _teacherService.GetItemByID(groupName);
                    if (teacher != null)
                    {
                        receiver = new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, true);
                    }
                    else
                    {
                        var student = _studentService.GetItemByID(groupName);
                        if (student != null)
                        {
                            receiver = new MemberGroupInfo(student.ID, student.Email, student.FullName, false);
                        }
                    }

                    if(receiver != null)
                    {
                        group = _groupService.CreateNewGroup(sender, receiver);
                    }

                }

                if (group != null)
                {

                    var listFile = _roxyFilemanHandler.UploadNewFeed(_userID, HttpContext);
                    List<FileManagerCore.Globals.MediaResponseModel> media = new List<FileManagerCore.Globals.MediaResponseModel>();
                    listFile?.TryGetValue("success", out media);
                    var item = new MessageEntity()
                    {
                        State = state,
                        Content = content,
                        Created = DateTime.Now,
                        Receiver = group.Name,
                        RemoveByAdmin = false,
                        Medias = media,
                        Sender = sender
                    };
                    if (!string.IsNullOrEmpty(title))
                    {
                        item.Title = title;
                    }
                    if (!string.IsNullOrEmpty(reply))
                    {
                        item.ReplyTo = reply;
                    }
                    _messageService.CreateMessage(item);

                    var needViews = group.Members.Select(o => o.ID)?.ToHashSet();
                    needViews.Remove(_userID);

                    var notification = new NotificationEntity()
                    {
                        GroupName = group.Name,
                        MessageCode = item.Code,
                        UserViews = needViews,
                        IsPrivated = group.IsPrivateChat
                    };

                    _notificationService.Create(notification);

                    return Success(new { message = item });
                }
            }

            return NotFoundData();
        }

        [HttpPut]
        public JsonResult UpdateMessage(string searchText)
        {
            if (IsAuthenticated())
            {
                if (!string.IsNullOrEmpty(searchText))
                {

                }
                else
                {

                }
            }

            return NotFoundData();
        }

        [HttpDelete]
        public JsonResult RemoveMessage(string code)
        {
            if (IsAuthenticated())
            {
                var item = _messageService.GetItemByCode(code);
                if(item != null)
                {
                    if(item.Sender == new MemberGroupInfo(_userID, _email, _name, _typeUser.Contains(Teacher)))
                    {
                        _messageService.Remove(item.ID);

                        return Success(new { message = item });
                    }
                    else
                    {
                        return ResponseApi(405, "Not permission");
                    }
                }
            }

            return NotFoundData();
        }

        #endregion
        #region Comment
        [HttpPost]
        public JsonResult CreateReply([FromForm]string groupName, [FromForm] string code, [FromForm] string content)
        {
            try
            {
                if (IsAuthenticated())
                {

                    var sender = new MemberGroupInfo(_userID, _email, _name, _typeUser.Contains(Teacher));
                    var group = _groupService.CreateQuery().Find(o => o.Name == groupName)?.FirstOrDefault();

                    if (group == null)
                    {

                        MemberGroupInfo receiver = null;
                        var teacher = _teacherService.GetItemByID(groupName);
                        if (teacher != null)
                        {
                            receiver = new MemberGroupInfo(teacher.ID, teacher.Email, teacher.FullName, true);
                        }
                        else
                        {
                            var student = _studentService.GetItemByID(groupName);
                            if (student != null)
                            {
                                receiver = new MemberGroupInfo(student.ID, student.Email, student.FullName, false);
                            }
                        }

                        if (receiver != null)
                        {
                            group = _groupService.CreateNewGroup(sender, receiver);
                        }

                    }

                    var listFile = _roxyFilemanHandler.UploadNewFeed(_userID, HttpContext);
                    List<FileManagerCore.Globals.MediaResponseModel> media = new List<FileManagerCore.Globals.MediaResponseModel>();
                    listFile?.TryGetValue("success", out media);
                    var item = new CommentEntity()
                    {
                        Content = content,
                        Created = DateTime.Now,
                        Medias = media,
                        Sender = sender,
                        ParentID = code
                    };
                    _commentService.CreateOrUpdate(item);

                    return Success(item);
                }
                else
                {
                    return NotFoundData();
                }
            }
            catch(Exception ex)
            {
                return Error(ex);
            }
        }
        [HttpGet]
        public async Task<JsonResult> GetReply(string code)
        {
            try
            {
                if (IsAuthenticated())
                {
                    var data = await _commentService.CreateQuery().FindAsync(o => o.ParentID == code);

                    return Success(data?.ToList());
                }
                else
                {
                    return NotFoundData();
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion

        #region Protect Func


        protected bool IsAuthenticated()
        {
            if (User == null) return false;
            if (User.Identity.IsAuthenticated)
            {
                _name = User.Identity.Name;
                _email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                _typeUser = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                _userID = User.FindFirst("UserID")?.Value;
            }
            return User.Identity.IsAuthenticated;
        }
        protected JsonResult ResponseApi(int Code, object Message , object Data=null)
        {
            return new JsonResult(new { Code, Message, Data });
        }
        protected JsonResult NotFoundData()
        {
            return ResponseApi(404, "Data Not Found");
        }
        protected JsonResult Error(Exception ex)
        {
            return ResponseApi(500, ex);
        }
        protected JsonResult Success(object Data)
        {
            return ResponseApi(200,"Success",Data);
        }
        #endregion

        #region notification
        [HttpGet]
        public JsonResult GetListNotification()
        {
            try
            {
                if (IsAuthenticated())
                {
                    var data = _notificationService.GetListNoViews(_userID);
                    if(data == null)
                    {
                        return NotFoundData();
                    }
                    else
                    {
                        return Success(data);
                    }
                }
                else
                {
                    return NotFoundData();
                }
            }
            catch(Exception ex)
            {
                return Error(ex);
            }

        }
        [HttpPost]
        public JsonResult UpdateViews(string groupName, string messageCode)
        {
            try
            {
                if (IsAuthenticated())
                {
                    var data = _notificationService.UpdateView(groupName,messageCode,_userID);
                    if (data == null)
                    {
                        return NotFoundData();
                    }
                    else
                    {
                        return Success(data);
                    }
                }
                else
                {
                    return NotFoundData();
                }
            }
            catch (Exception ex)
            {
                return Error(ex);
            }
        }
        #endregion

        #region convert newfeedold to message 
        public int ConvertNewFeed()
        {
            var all = _newFeedService.CreateQuery().Find(o => true)?.ToList();
            if (all == null) return 0;
            int count = 0;
            for (int i = 0; i < all.Count; i++)
            {
                NewFeedEntity item = all[i];
                if (item.Receivers?.FirstOrDefault() == null) continue;
                //public string Poster { get; set; }
                //public string PosterName { get; set; }
                //public List<object> Likes { get; set; }
                //public List<object> UnLikes { get; set; }
                //public int ReplyCount { get; set; }
                //public int FeedType { get; set; }
                //public string Name { get; set; }

                //public string Code { get; set; } = Guid.NewGuid().ToString();
                //public object Sender { get; set; }
                //public HashSet<string> Receivers { get; set; } = new HashSet<string>();
                //public string Receiver { get; set; } // groupName
                //public string Title { get; set; }
                //public string Content { get; set; }
                //public List<FileManagerCore.Globals.MediaResponseModel> Medias { get; set; } = new List<FileManagerCore.Globals.MediaResponseModel>();
                //public int State { get; set; } = 0;
                //public HashSet<string> Views { get; set; }
                //public string ReplyTo { get; set; } // code message
                //public bool? RemoveByAdmin { get; set; } = false;
                //public DateTime? Created { get; set; } = DateTime.Now;
                //public DateTime? Updated { get; set; } = DateTime.Now;

                var student = item.Sender == null ? _studentService.GetItemByID(item.Poster) : _studentService.CreateQuery().Find(o => o.Email == item.Sender.ToString())?.FirstOrDefault();
                var teacher = item.Sender == null ? _teacherService.GetItemByID(item.Poster) : _teacherService.CreateQuery().Find(o => o.Email == item.Sender.ToString())?.FirstOrDefault();
                if (student == null && teacher == null) continue;
                var sender = student == null
                    ?
                new MemberGroupInfo(teacher.ID,teacher.Email,teacher.FullName,true)
                    :
                new MemberGroupInfo(student.ID,student.Email,student.FullName,false);
                var newItem = new MessageEntity()
                {
                    Code = item.ID,
                    Content = item.Content,
                    Medias = item.Medias,
                    Receiver = item.Receivers?.FirstOrDefault(),
                    RemoveByAdmin = item.RemoveByAdmin,
                    Sender = sender,
                    Title = item.Title,
                    State = 1,
                    Views = new HashSet<string>() { sender.ID },
                    Created = item.TimePost > DateTime.MinValue ? item.TimePost : item.Created
                };
                if (_messageService.GetItemByCode(newItem.Code) == null)
                {
                    _messageService.CreateOrUpdate(newItem);
                    count++;
                    //_newFeedService.Remove(item.ID);
                }

            }

            return count;
        }
        #endregion
    }

}