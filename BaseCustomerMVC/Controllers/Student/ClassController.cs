﻿using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Controllers.Student
{
    public class ClassController : StudentController
    {
        private readonly ClassService _classService;
        private readonly StudentService _studentService;
        private readonly CenterService _centerService;
        private readonly CourseService _courseService;
        private readonly SubjectService _subjectService;
        private readonly SkillService _skillService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly CourseHelper _courseHelper;
        //private readonly MailHelper _mailHelper;
        private readonly LessonScheduleService _lessonScheduleService;
        private readonly ChapterService _chapterService;
        private readonly LessonHelper _lessonHelper;
        private readonly ProgressHelper _progressHelper;
        private readonly ExamService _examService;

        public ClassController(
            ClassService classService,
            StudentService studentService,
            CenterService centerService,
            CourseService courseService,
            SubjectService subjectService,
            SkillService skillService,
            ClassSubjectService classSubjectService,
            CourseHelper courseHelper,
            //MailHelper mailHelper
            ChapterService chapterService,
            LessonHelper lessonHelper,
            ProgressHelper progressHelper,
            ExamService examService
        )
        {
            _classService = classService;
            _studentService = studentService;
            _centerService = centerService;
            _courseService = courseService;
            _subjectService = subjectService;
            _skillService = skillService;
            _classSubjectService = classSubjectService;
            _courseHelper = courseHelper;
            //_mailHelper = mailHelper;
            _chapterService = chapterService;
            _lessonHelper = lessonHelper;
            _progressHelper = progressHelper;
            _examService = examService;
        }

        #region add to my personal class
        public async Task<JsonResult> CreateClass(List<ClassSubjectEntity> classSubjects = null)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(userId);
            var center = _centerService.GetItemByCode("eduso");//mặc định sẽ thêm vào cơ sở eduso

            if (student == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Error", "Vui lòng đăng nhập lại" },
                            {"Status",false }
                        });
            }

            var MyClass = _classService.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, student.ID);
            if (MyClass == null)
            {
                var @class = new ClassEntity();
                @class.Name = $"Lớp học của {student.FullName}";
                @class.Created = DateTime.Now;
                @class.TeacherID = student.ID; // creator
                @class.Skills = new List<string>();
                @class.Subjects = new List<string>();
                //@class.Members = new List<ClassMemberEntity> { new ClassMemberEntity { TeacherID = userId, Type = ClassMemberType.OWNER, Name = cm.FullName } };
                @class.TotalLessons = 0;
                @class.TotalPractices = 0;
                @class.TotalExams = 0;
                @class.IsActive = true;
                @class.Center = center.ID;
                @class.StartDate = DateTime.Now;
                @class.EndDate = DateTime.Now.AddYears(99);
                @class.ClassMechanism = CLASS_MECHANISM.PERSONAL;

                _classService.Save(@class);

                //add student to class
                await AddStudent(@class.ID, student);

                //Create class subjects
                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var csubject in classSubjects)
                    {
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(csubject, @class, student, out newMember, out lessoncount, out examcount, out practicecount);
                        if (!@class.Skills.Contains(csubject.SkillID))
                            @class.Skills.Add(csubject.SkillID);
                        if (!@class.Subjects.Contains(csubject.SubjectID))
                            @class.Subjects.Add(csubject.SubjectID);
                        //if (!@class.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                        //    @class.Members.Add(newMember);
                        @class.TotalLessons += lessoncount;
                        @class.TotalExams += examcount;
                        @class.TotalPractices += practicecount;
                        //var skill = _skillService.GetItemByID(csubject.SkillID);
                        //if (skill == null) continue;
                        //var course = _courseService.GetItemByID(csubject.CourseID);
                    }
                }

                Dictionary<string, object> response = new Dictionary<string, object>()
                {
                    {"Data",@class },
                    {"Error",null },
                    {"Msg","Success" }
                };
                return Json(response);
            }
            else
            {
                var processCS = new List<string>();
                var oldData = _classService.GetItemByID(MyClass.ID);
                if (oldData == null) return new JsonResult(new Dictionary<string, object>()
                    {
                        {"Error", "Không tìm thấy lớp" }
                    });

                oldData.Updated = DateTime.Now;

                oldData.Skills = new List<string>();
                oldData.Subjects = new List<string>();
                //var creator = _teacherService.GetItemByID(oldData.TeacherID);
                //oldData.Members = new List<ClassMemberEntity> { };
                //if (creator != null)
                //    oldData.Members.Add(new ClassMemberEntity { TeacherID = creator.ID, Type = ClassMemberType.TEACHER, Name = creator.FullName });
                oldData.TotalLessons = 0;
                oldData.TotalExams = 0;
                oldData.TotalPractices = 0;
                //oldData.ClassMechanism = item.ClassMechanism;

                var oldSubjects = _classSubjectService.GetByClassID(MyClass.ID);

                if (oldSubjects != null)
                {
                    foreach (var oSbj in oldSubjects)
                    {
                        var nSbj = classSubjects.Find(t => t.ID == oSbj.ID);
                        if (nSbj == null || (nSbj.CourseID != oSbj.CourseID))
                        //delete oldSubject
                        {
                            _ = RemoveClassSubject(oSbj);
                            if (nSbj != null)
                                nSbj.ID = null;//remove ID to create new
                        }

                        if (nSbj != null)
                        {
                            var newMember = new ClassMemberEntity();
                            long lessoncount = 0;
                            long examcount = 0;
                            long practicecount = 0;

                            if (nSbj.CourseID != oSbj.CourseID)//SkillID ~ CourseID
                            {
                                nSbj.ID = CreateNewClassSubject(nSbj, oldData, student, out newMember, out lessoncount, out examcount, out practicecount);
                                if (string.IsNullOrEmpty(nSbj.ID))//Error
                                    continue;
                            }
                            else //Not change
                            {
                                //update period
                                oSbj.StartDate = MyClass.StartDate.ToUniversalTime();
                                oSbj.EndDate = MyClass.EndDate.ToUniversalTime();
                                oSbj.TypeClass = nSbj.TypeClass;

                                _classSubjectService.Save(oSbj);

                                examcount = oSbj.TotalExams;
                                lessoncount = oSbj.TotalLessons;
                                practicecount = oSbj.TotalPractices;
                            }

                            processCS.Add(nSbj.ID);
                            if (!oldData.Skills.Contains(nSbj.SkillID))
                                oldData.Skills.Add(nSbj.SkillID);
                            if (!oldData.Subjects.Contains(nSbj.SubjectID))
                                oldData.Subjects.Add(nSbj.SubjectID);


                            oldData.TotalLessons += lessoncount;
                            oldData.TotalExams += examcount;
                            oldData.TotalPractices += practicecount;
                        }
                    }
                }

                if (classSubjects != null && classSubjects.Count > 0)
                {
                    foreach (var nSbj in classSubjects)
                    {
                        if (processCS.IndexOf(nSbj.ID) >= 0)
                            continue;
                        //create new subject
                        var newMember = new ClassMemberEntity();
                        long lessoncount = 0;
                        long examcount = 0;
                        long practicecount = 0;
                        var nID = CreateNewClassSubject(nSbj, oldData, student, out newMember, out lessoncount, out examcount, out practicecount);
                        if (string.IsNullOrEmpty(nSbj.ID))//Error
                            continue;

                        if (!oldData.Skills.Contains(nSbj.SkillID))
                            oldData.Skills.Add(nSbj.SkillID);
                        if (!oldData.Subjects.Contains(nSbj.SubjectID))
                            oldData.Subjects.Add(nSbj.SubjectID);
                        //if (!oldData.Members.Any(t => t.TeacherID == newMember.TeacherID && t.Type == ClassMemberType.TEACHER))
                        //    oldData.Members.Add(newMember);
                        oldData.TotalLessons += lessoncount;
                        oldData.TotalExams += examcount;
                        oldData.TotalPractices += practicecount;
                    }
                }

                //update data
                _classService.Save(oldData);

                Dictionary<string, object> response = new Dictionary<string, object>()
                    {
                        {"Data",MyClass },
                        {"Error",null },
                        {"Msg","Success" }
                    };
                return new JsonResult(response);
            }

            //return null;
        }

        private async Task RemoveClassSubject(ClassSubjectEntity cs)
        {
            ////remove old schedule
            var CsTask = _lessonScheduleService.RemoveClassSubject(cs.ID);
            //remove chapter
            var CtTask = _chapterService.RemoveClassSubjectChapter(cs.ID);
            //remove clone lesson
            var LsTask = _lessonHelper.RemoveClassSubjectLesson(cs.ID);
            //remove progress: learning history => class progress, chapter progress, lesson progress
            var LhTask = _progressHelper.RemoveClassSubjectHistory(cs.ID);
            //remove exam
            var ExTask = _examService.RemoveClassSubjectExam(cs.ID);
            //remove classSubject
            //await Task.WhenAll(CsTask, CtTask, LsTask, LhTask, ExTask, ExDetailTask);
            await _classSubjectService.RemoveAsync(cs.ID);
        }

        private string CreateNewClassSubject(ClassSubjectEntity nSbj, ClassEntity @class, StudentEntity student, out ClassMemberEntity member, out long lessoncount, out long examcount, out long practicecount, bool notify = true)
        {
            member = new ClassMemberEntity();
            lessoncount = 0;
            examcount = 0;
            practicecount = 0;
            try
            {
                var subject = _subjectService.GetItemByID(nSbj.SubjectID);
                if (subject == null)
                {
                    throw new Exception("Subject " + nSbj.SubjectID + " is not avaiable");
                }
                var course = _courseService.GetItemByID(nSbj.CourseID);
                if (course == null || !course.IsActive)
                {
                    throw new Exception("Course " + nSbj.CourseID + " is not avaiable");
                }

                lessoncount = course.TotalLessons;
                examcount = course.TotalExams;
                practicecount = course.TotalPractices;

                //var teacher = _teacherService.GetItemByID(nSbj.TeacherID);
                //if (teacher == null || !teacher.IsActive || !teacher.Subjects.Contains(nSbj.SubjectID))
                //{
                //    throw new Exception("Teacher " + nSbj.TeacherID + " is not avaiable");
                //}

                nSbj.ClassID = @class.ID;
                nSbj.StartDate = @class.StartDate;
                nSbj.EndDate = @class.EndDate;
                nSbj.SkillID = course.SkillID;
                nSbj.Description = course.Description;
                nSbj.LearningOutcomes = course.LearningOutcomes;
                nSbj.TotalLessons = course.TotalLessons;
                nSbj.TotalPractices = course.TotalPractices;
                nSbj.TotalExams = course.TotalExams;

                var skill = _skillService.GetItemByID(nSbj.SkillID);

                var center = _centerService.GetItemByID(@class.Center);

                _classSubjectService.Save(nSbj);

                member = new ClassMemberEntity
                {
                    Name = $"Lớp của {student.FullName}",
                    TeacherID = $"gv{student.ID}",
                    Type = ClassMemberType.TEACHER //????
                };
                //Clone Course
                _courseHelper.CloneForClassSubject(nSbj);

                //if (notify)
                //    _ = _mailHelper.SendTeacherJoinClassNotify(student.FullName, student.Email, @class.Name, skill?.Name, @class.StartDate, @class.EndDate, center.Name);
                return nSbj.ID;
            }
            catch (Exception e)
            {
                return "";
            }
        }

        public async Task<string> AddStudent(string ClassID, StudentEntity student)
        {
            if (string.IsNullOrEmpty(ClassID))
                return "Lớp không tồn tại";
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
                return "Lớp không tồn tại";

            if (student == null)
                return "Học viên không tồn tại";
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
                return "Học viên đã được thêm vào lớp";
            return "Có lỗi, vui lòng thực hiện lại";
        }

        public async Task<JsonResult> AddToPersonalClass(string CourseID)
        {
            var userId = User.Claims.GetClaimByType("UserID").Value;
            var student = _studentService.GetItemByID(userId);
            var course = _courseService.GetItemByID(CourseID);
            var MyClass = _classService.GetClassByMechanism(CLASS_MECHANISM.PERSONAL, student.ID);

            if (course == null)
            {
                return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Msg", "Không tìm thấy tài liệu." },
                            {"Status",false }
                        });
            }
            else
            {
                //var SkillID = course.SkillID;//Môn học
                //var GradeID = course.GradeID;//Cấp độ
                //var SubjectID = course.SubjectID;//Chương trình

                var oldSubjects = _classSubjectService.GetByClassID(MyClass.ID);
                if (oldSubjects.Find(x => x.CourseID == CourseID) != null)
                {
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Msg", "Bạn đã có tài liệu này rồi." },
                            {"Status",false }
                        });
                }
                else
                {
                    var classSubject = new ClassSubjectEntity();
                    classSubject.CourseID = CourseID;
                    classSubject.SkillID = course.SkillID;
                    classSubject.GradeID = course.GradeID;
                    classSubject.SubjectID = course.SubjectID;
                    classSubject.TeacherID = student.ID;
                    //classSubject.TypeClass = CLASS_TYPE.EXTEND;

                    oldSubjects.Add(classSubject);

                    await CreateClass(oldSubjects);
                    return new JsonResult(new Dictionary<string, object>()
                        {
                            {"Msg", $"Thêm tài liệu vào {MyClass.Name} thành công." },
                            {"Status",true }
                        });
                }
            }
        }
        #endregion
    }
}
