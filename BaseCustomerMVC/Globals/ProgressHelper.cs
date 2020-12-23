using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class ProgressHelper
    {
        private readonly ClassProgressService _classProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly LearningHistoryService _learningHistoryService;

        private readonly LessonService _lessonService;
        private readonly ChapterService _chapterService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ClassService _classService;

        private readonly ExamService _examService;
        private readonly ExamDetailService _examDetailService;
        private readonly StudentService _studentService;

        private readonly LessonScheduleService _lessonScheduleService;

        public ProgressHelper(
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ChapterProgressService chapterProgressService,
            LessonProgressService lessonProgressService,
            LearningHistoryService learningHistoryService,

            LessonService lessonService,
            ChapterService chapterService,
            ClassSubjectService classSubjectService,
            ClassService classService,

            ExamService examService,
            ExamDetailService examDetailService,
            LessonScheduleService lessonScheduleService,
            StudentService studentService
        )
        {
            _learningHistoryService = learningHistoryService;

            _lessonProgressService = lessonProgressService;
            _chapterProgressService = chapterProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _classProgressService = classProgressService;

            _lessonService = lessonService;
            _chapterService = chapterService;
            _classSubjectService = classSubjectService;
            _classService = classService;

            _examService = examService;
            _examDetailService = examDetailService;
            _lessonScheduleService = lessonScheduleService;
            _studentService = studentService;
        }

        #region Learning Progress
        public async Task CreateHist(LearningHistoryEntity item)
        {
            var historycount = _learningHistoryService.CountHistory(item);
            item.Time = DateTime.UtcNow;
            item.ViewCount = (int)historycount;
            _learningHistoryService.Save(item);

            await UpdateLessonLastLearn(item);
        }

        public Task RemoveClassHistory(string ClassID)
        {
            _ = _learningHistoryService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _classProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _classSubjectProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            _ = _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID);
            return Task.CompletedTask;
        }

        public async Task RemoveClassHistory(string[] ClassIDs)
        {
            await _learningHistoryService.CreateQuery().DeleteManyAsync(o => ClassIDs.Contains(o.ClassID));
            await _classProgressService.CreateQuery().DeleteManyAsync(t => ClassIDs.Contains(t.ClassID));
            await _chapterProgressService.CreateQuery().DeleteManyAsync(t => ClassIDs.Contains(t.ClassID));
            await _classSubjectProgressService.CreateQuery().DeleteManyAsync(t => ClassIDs.Contains(t.ClassID));
            await _lessonProgressService.CreateQuery().DeleteManyAsync(t => ClassIDs.Contains(t.ClassID));
        }

        public Task RemoveClassStudentHistory(string ClassID, string StudentID)
        {
            _ = _learningHistoryService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            _ = _classProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            _ = _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            _ = _classSubjectProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);
            _ = _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassID == ClassID && t.StudentID == StudentID);

            _ = _examService.RemoveClassStudentExam(ClassID, StudentID);

            return Task.CompletedTask;
        }




        public async Task RemoveClassSubjectHistory(string ClassSubjectID)
        {
            await _learningHistoryService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            var subjectProgresses = _classSubjectProgressService.GetListOfCurrentSubject(ClassSubjectID);
            if (subjectProgresses != null)
                foreach (var progress in subjectProgresses)
                    await DecreaseClassSubject(progress);//remove subject progress from class progress
            await _classSubjectProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            await _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            await _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
        }

        private async Task DecreaseClassSubject(ClassSubjectProgressEntity clssbj)
        {
            var update = new UpdateDefinitionBuilder<ClassProgressEntity>()
                     //.AddToSet(t => t.CompletedLessons, item.ClassSubjectID)
                     .Inc(t => t.Completed, 0 - clssbj.Completed)
                     .Inc(t => t.ExamDone, 0 - clssbj.ExamDone)
                     .Inc(t => t.TotalPoint, 0 - clssbj.TotalPoint)
                     .Inc(t => t.PracticePoint, 0 - clssbj.PracticePoint)
                     .Inc(t => t.PracticeDone, 0 - clssbj.PracticeDone);
            await _classProgressService.Collection.UpdateManyAsync(t => t.ClassID == clssbj.ClassID && t.StudentID == clssbj.StudentID, update);
        }

        public async Task UpdateLessonLastLearn(LearningHistoryEntity item, bool incComplete = false)
        {
            var currentProgress = _lessonProgressService.GetByStudentID_LessonID(item.StudentID, item.LessonID);
            if (currentProgress == null)
            {
                currentProgress = new LessonProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = item.ChapterID,
                    LessonID = item.LessonID,
                    StudentID = item.StudentID,
                    LastDate = item.Time,
                    FirstDate = item.Time,
                    TotalLearnt = 1,
                };
                await _lessonProgressService.Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                currentProgress.LastDate = DateTime.Now;
                await _lessonProgressService.Collection.UpdateManyAsync(t => t.StudentID == item.StudentID && t.LessonID == item.LessonID,
                     new UpdateDefinitionBuilder<LessonProgressEntity>()
                     .Inc(t => t.TotalLearnt, 1)
                     .Set(t => t.LastDate, item.Time)
                     );
            }

            if (item.ChapterID != "0")
                await UpdateChapterLastLearn(currentProgress, currentProgress.TotalLearnt == 1);
            else
                await UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = currentProgress.LessonID, ClassSubjectID = currentProgress.ClassSubjectID, ClassID = currentProgress.ClassID, LastDate = currentProgress.LastDate }, currentProgress.TotalLearnt == 1);
        }

        public async Task UpdateChapterLastLearn(LessonProgressEntity item, bool incComplete = false)
        {
            var currentProgress = _chapterProgressService.GetItemByChapterID(item.ChapterID, item.StudentID);
            var chapter = _chapterService.GetItemByID(item.ChapterID);
            if (chapter == null)
                return;
            if (currentProgress == null)
            {
                currentProgress = new ChapterProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = item.ChapterID,
                    StudentID = item.StudentID,
                    LastLessonID = item.LessonID,
                    LastDate = item.LastDate,
                    Completed = 1,
                };
                await _chapterProgressService.Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var needUpdate = false;
                var update = Builders<ChapterProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ChapterProgressEntity>>();

                if (currentProgress.LastDate < item.LastDate)
                {
                    needUpdate = true;
                    updates.Add(update.Set(t => t.LastDate, item.LastDate).Set(t => t.LastLessonID, item.LessonID));
                }

                if (item.TotalLearnt == 1 || incComplete)
                {
                    needUpdate = true;
                    updates.Add(update.Inc(t => t.Completed, 1));
                }

                if (needUpdate)
                {
                    var filter = Builders<ChapterProgressEntity>.Filter.Where(t => t.StudentID == item.StudentID && t.ChapterID == item.ChapterID);
                    await _chapterProgressService.Collection.UpdateManyAsync(filter, Builders<ChapterProgressEntity>.Update.Combine(updates));
                }
                else
                    return; //No more update need

            }

            if (chapter.ParentID != "0")
                await UpdateParentChapterLastLearn(new ChapterProgressEntity { LastLessonID = item.LessonID, ChapterID = chapter.ParentID, ClassSubjectID = item.ClassSubjectID, ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate }, incComplete);
            else
                await UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = item.LessonID, ClassSubjectID = item.ClassSubjectID, ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate }, incComplete);
        }

        public async Task UpdateParentChapterLastLearn(ChapterProgressEntity item, bool incComplete = false)
        {
            var chapter = _chapterService.GetItemByID(item.ChapterID);
            if (chapter == null)
                return;

            var currentProgress = _chapterProgressService.GetItemByChapterID(item.ChapterID, item.StudentID);
            if (currentProgress == null)
            {
                currentProgress = new ChapterProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    ChapterID = item.ParentID,
                    StudentID = item.StudentID,
                    LastLessonID = item.LastLessonID,
                    LastDate = item.LastDate,
                    Completed = 1
                };
                await _chapterProgressService.Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var needUpdate = false;
                var update = Builders<ChapterProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ChapterProgressEntity>>();

                if (currentProgress.LastDate < item.LastDate)
                {
                    needUpdate = true;
                    updates.Add(update.Set(t => t.LastDate, item.LastDate).Set(t => t.LastLessonID, item.LastLessonID));
                }

                if (incComplete)
                {
                    needUpdate = true;
                    updates.Add(update.Inc(t => t.Completed, 1));
                }

                if (needUpdate)
                {
                    var filter = Builders<ChapterProgressEntity>.Filter.Where(t => t.StudentID == item.StudentID && t.ChapterID == item.ChapterID);
                    await _chapterProgressService.Collection.UpdateManyAsync(filter, Builders<ChapterProgressEntity>.Update.Combine(updates));
                }
                else
                    return; //No more update need
            }

            if (chapter.ParentID != "0")
                await UpdateParentChapterLastLearn(new ChapterProgressEntity { LastLessonID = item.LastLessonID, ChapterID = chapter.ParentID, ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate }, incComplete);
            else
                await UpdateClassSubjectLastLearn(new ClassSubjectProgressEntity { LastLessonID = item.LastLessonID, ClassSubjectID = item.ClassSubjectID, ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate }, incComplete);
        }

        public async Task UpdateClassSubjectLastLearn(ClassSubjectProgressEntity item, bool incComplete = false)
        {
            var currentSbj = _classSubjectService.GetItemByID(item.ClassSubjectID);
            if (currentSbj == null)
                return;

            var currentProgress = _classSubjectProgressService.GetItemByClassSubjectID(item.ClassSubjectID, item.StudentID);
            if (currentProgress == null)
            {
                currentProgress = new ClassSubjectProgressEntity
                {
                    ClassID = item.ClassID,
                    ClassSubjectID = item.ClassSubjectID,
                    StudentID = item.StudentID,
                    LastLessonID = item.LastLessonID,
                    LastDate = item.LastDate,
                    Completed = 1
                };
                await _classSubjectProgressService.Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var needUpdate = false;
                var update = Builders<ClassSubjectProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ClassSubjectProgressEntity>>();

                if (currentProgress.LastDate < item.LastDate)
                {
                    needUpdate = true;
                    updates.Add(update.Set(t => t.LastDate, item.LastDate).Set(t => t.LastLessonID, item.LastLessonID));
                }

                if (incComplete)
                {
                    needUpdate = true;
                    updates.Add(update.Inc(t => t.Completed, 1));
                }

                if (needUpdate)
                {
                    var filter = Builders<ClassSubjectProgressEntity>.Filter.Where(t => t.StudentID == item.StudentID && t.ClassSubjectID == item.ClassSubjectID);
                    await _classSubjectProgressService.Collection.UpdateManyAsync(filter, Builders<ClassSubjectProgressEntity>.Update.Combine(updates));
                }
                else
                    return; //No more update need
            }

            await UpdateClassLastLearn(new ClassProgressEntity { ClassID = item.ClassID, StudentID = item.StudentID, LastDate = item.LastDate }, incComplete);
        }

        public async Task UpdateClassLastLearn(ClassProgressEntity item, bool incComplete = false)
        {
            var currentClass = _classService.GetItemByID(item.ClassID);
            if (currentClass == null)
                return;

            var currentProgress = _classProgressService.GetItemByClassID(item.ClassID, item.StudentID);
            if (currentProgress == null)
            {
                currentProgress = new ClassProgressEntity
                {
                    ClassID = item.ClassID,
                    StudentID = item.StudentID,
                    LastLessonID = item.LastLessonID,
                    LastDate = item.LastDate,
                    Completed = 1
                };
                await _classProgressService.Collection.InsertOneAsync(currentProgress);
            }
            else
            {
                var needUpdate = false;
                var update = Builders<ClassProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ClassProgressEntity>>();

                if (currentProgress.LastDate < item.LastDate)
                {
                    needUpdate = true;
                    updates.Add(update.Set(t => t.LastDate, item.LastDate).Set(t => t.LastLessonID, item.LastLessonID));
                }

                if (incComplete)
                {
                    needUpdate = true;
                    updates.Add(update.Inc(t => t.Completed, 1));
                }

                if (needUpdate)
                {
                    var filter = Builders<ClassProgressEntity>.Filter.Where(t => t.StudentID == item.StudentID && t.ClassID == item.ClassID);
                    await _classProgressService.Collection.UpdateManyAsync(filter, Builders<ClassProgressEntity>.Update.Combine(updates));
                }
            }
        }
        #endregion

        #region Learning Result
        public async Task<bool> ResetLesssonPoint(LessonEntity lesson, string studentID)
        {
            var result = false;
            var lessonProgress = _lessonProgressService.GetByStudentID_LessonID(studentID, lesson.ID);
            if (lessonProgress != null)
            {

                var incPoint = 0.0;
                long incCount = 0;
                var incPracPoint = 0.0;
                long incPracCount = 0;

                var pointChange = 0 - lesson.Point;

                if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                    incPoint = pointChange;
                else
                    incPracPoint = pointChange;


                if (lesson.ChapterID != "0")
                    await UpdateParentChapPoint(lesson.ChapterID, studentID, incPoint, incCount, incPracPoint, incPracCount);
                else
                    await UpdateClassSubjectPoint(lesson.ChapterID, studentID, incPoint, incCount, incPracPoint, incPracCount);

                //UpdateChapterPoint()

                //if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                //{
                //    _chapterProgressService.DecreasePoint(lessonProgress);
                //    _classSubjectProgressService.DecreasePoint(lessonProgress);
                //    _classProgressService.DecreasePoint(lessonProgress);
                //}
                //else
                //{
                //    _chapterProgressService.DecreasePracticePoint(lessonProgress);
                //    _classSubjectProgressService.DecreasePracticePoint(lessonProgress);
                //    _classProgressService.DecreasePracticePoint(lessonProgress);
                //}
                _lessonProgressService.ResetPoint(lessonProgress);
            }
            return result;
        }

        public async Task<LessonProgressEntity> UpdateLessonPoint(ExamEntity exam, bool incCounter = false)
        {
            //var pointchange = exam.MaxPoint > 0 ? (exam.Point - exam.LastPoint) * 100 / exam.MaxPoint : 0;

            var prg = await _lessonProgressService.UpdatePoint(exam);

            if (prg.ChapterID != "0")
                _ = UpdateChapterPoint(prg, incCounter: incCounter);
            else
                _ = UpdateClassSubjectPoint(prg, incCounter: incCounter);

            return prg;
        }

        public async Task UpdateChapterPoint(LessonProgressEntity item, double pointchange = 0, bool incCounter = false)
        {
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null)
            {
                return;
            }
            var chapter = _chapterService.GetItemByID(lesson.ChapterID);
            if (chapter == null)
            {
                return;
            }
            var progress = _chapterProgressService.GetItemByChapterID(chapter.ID, item.StudentID);
            if (progress == null)//progress not found => create progress
            {
                progress = _chapterProgressService.NewProgressEntity(chapter, item.StudentID);
                _chapterProgressService.Save(progress);
                //return;
            }
            //else
            //{
            var point = (pointchange != 0 ? pointchange : item.PointChange) * item.Multiple;

            var incPoint = 0.0;
            long incCount = 0;
            var incPracPoint = 0.0;
            long incPracCount = 0;

            if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
            {
                incPoint = point;

                if (incCounter)//new
                    incCount = (long)item.Multiple;

                progress.ExamDone += incCount;
                progress.TotalPoint += incPoint;
                progress.AvgPoint = progress.TotalPoint / progress.ExamDone;

                await _chapterProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                    Builders<ChapterProgressEntity>.Update
                    .Inc(t => t.ExamDone, incCount)
                    .Inc(t => t.TotalPoint, incPoint)
                    .Set(t => t.AvgPoint, progress.AvgPoint)
                    );
            }
            else
            {
                incPracPoint = point;

                if (incCounter)//new
                    incPracCount = (long)item.Multiple;

                progress.PracticeDone += incPracCount;
                progress.PracticePoint += incPracPoint;
                progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;

                await _chapterProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                    Builders<ChapterProgressEntity>.Update
                    .Inc(t => t.PracticeDone, incPracCount)
                    .Inc(t => t.PracticePoint, incPracPoint)
                    .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                    );
            }

            if (chapter.ParentID != "0")
                await UpdateParentChapPoint(chapter.ParentID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
            else
                await UpdateClassSubjectPoint(chapter.ClassSubjectID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
            //}
        }

        public async Task UpdateParentChapPoint(string ChapterID, string StudentID, double incPoint, long incCount, double incPracPoint, long incPracCount)
        {
            var chapter = _chapterService.GetItemByID(ChapterID);
            if (chapter == null)
            {
                return;
            }
            var progress = _chapterProgressService.GetItemByChapterID(chapter.ID, StudentID);
            if (progress == null)//progress not found => create progress
            {
                progress = new ChapterProgressEntity
                {
                    ChapterID = chapter.ID,
                    StudentID = StudentID,
                    ClassID = chapter.ClassID,
                    ClassSubjectID = chapter.ClassSubjectID,
                };
                _chapterProgressService.Save(progress);
                //return;
            }
            //else
            //{

            var update = Builders<ChapterProgressEntity>.Update;
            var updates = new List<UpdateDefinition<ChapterProgressEntity>>();

            if (incCount != 0 || incPoint != 0)
            {
                progress.ExamDone += incCount;
                progress.TotalPoint += incPoint;
                progress.AvgPoint = progress.ExamDone != 0 ? progress.TotalPoint / progress.ExamDone : 0;

                updates.Add(update.Inc(t => t.ExamDone, incCount)
                      .Inc(t => t.TotalPoint, incPoint)
                      .Set(t => t.AvgPoint, progress.AvgPoint));
            }

            if (incPracCount != 0 || incPracPoint != 0)
            {
                progress.PracticeDone += incPracCount;
                progress.PracticePoint += incPracPoint;

                progress.PracticeAvgPoint = progress.PracticeDone != 0 ? progress.PracticePoint / progress.PracticeDone : 0;

                updates.Add(update.Inc(t => t.PracticeDone, incPracCount)
                    .Inc(t => t.PracticePoint, incPracPoint)
                    .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                    );
            }

            if (updates.Count > 0)
            {
                await _chapterProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID, update.Combine(updates));

                if (chapter.ParentID != "0")
                    await UpdateParentChapPoint(chapter.ParentID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
                else
                    await UpdateClassSubjectPoint(chapter.ClassSubjectID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
            }
            //}
        }

        public async Task UpdateClassSubjectPoint(LessonProgressEntity item, double pointchange = 0, bool incCounter = false)
        {
            var lesson = _lessonService.GetItemByID(item.LessonID);
            if (lesson == null)
            {
                return;
            }
            var classSbj = _classSubjectService.GetItemByID(item.ClassSubjectID);
            if (classSbj == null)
            {
                return;
            }
            var progress = _classSubjectProgressService.GetItemByClassSubjectID(classSbj.ID, item.StudentID);
            if (progress == null)
            {
                return;
            }
            else
            {
                var point = (pointchange != 0 ? pointchange : item.PointChange) * item.Multiple;

                var incPoint = 0.0;
                long incCount = 0;
                var incPracPoint = 0.0;
                long incPracCount = 0;

                if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                {
                    incPoint = point;

                    //if (item.Tried == 1 || progress.ExamDone == 0)//new
                    if (incCounter)
                        incCount = (long)item.Multiple;

                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;
                    progress.AvgPoint = progress.ExamDone != 0 ? progress.TotalPoint / progress.ExamDone : 0;

                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassSubjectProgressEntity>.Update
                        .Inc(t => t.ExamDone, incCount)
                        .Inc(t => t.TotalPoint, incPoint)
                        .Set(t => t.AvgPoint, progress.AvgPoint)
                        );
                }
                else
                {
                    incPracPoint = point;
                    //if (item.Tried == 1 || progress.PracticeDone == 0)//new
                    if (incCounter)
                        incPracCount = (long)item.Multiple;

                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;
                    progress.PracticeAvgPoint = progress.PracticeDone != 0 ? progress.PracticePoint / progress.PracticeDone : 0;

                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassSubjectProgressEntity>.Update
                        .Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );
                }

                await UpdateClassPoint(classSbj.ClassID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
            }
        }

        public async Task UpdateClassSubjectPoint(string ClassSubjectID, string StudentID, double incPoint, long incCount, double incPracPoint, long incPracCount)
        {
            var classSbj = _classSubjectService.GetItemByID(ClassSubjectID);
            if (classSbj == null)
            {
                return;
            }
            var progress = _classSubjectProgressService.GetItemByClassSubjectID(ClassSubjectID, StudentID);
            if (progress == null)
            {
                return;
            }
            else
            {
                var update = Builders<ClassSubjectProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ClassSubjectProgressEntity>>();


                if (incCount != 0 || incPoint != 0)
                {
                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;
                    progress.AvgPoint = progress.ExamDone != 0 ? progress.TotalPoint / progress.ExamDone : 0;

                    updates.Add(update.Inc(t => t.ExamDone, incCount)
                        .Inc(t => t.TotalPoint, incPoint)
                        .Set(t => t.AvgPoint, progress.AvgPoint));

                    //await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                    //    Builders<ClassSubjectProgressEntity>.Update
                    //    .Inc(t => t.ExamDone, incCount)
                    //    .Inc(t => t.TotalPoint, incPoint)
                    //    .Set(t => t.AvgPoint, progress.AvgPoint)
                    //    );
                }

                if (incPracCount != 0 || incPracPoint != 0)
                {
                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;

                    progress.PracticeAvgPoint = progress.PracticeDone != 0 ? progress.PracticePoint / progress.PracticeDone : 0;

                    updates.Add(update.Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );

                    //await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                    //    Builders<ClassSubjectProgressEntity>.Update
                    //    .Inc(t => t.PracticeDone, incPracCount)
                    //    .Inc(t => t.PracticePoint, incPracPoint)
                    //    .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                    //    );
                }
                if (updates.Count > 0)
                {
                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID, update.Combine(updates));
                    await UpdateClassPoint(classSbj.ClassID, StudentID, incPoint, incCount, incPracPoint, incPracCount);
                }
            }
        }

        public async Task UpdateClassPoint(string ClassID, string StudentID, double incPoint, long incCount, double incPracPoint, long incPracCount)
        {
            var @class = _classService.GetItemByID(ClassID);
            if (@class == null)
            {
                return;
            }
            var progress = _classProgressService.GetItemByClassID(ClassID, StudentID);
            if (progress == null)
            {
                return;
            }
            else
            {
                progress.PracticeDone += incPracCount;
                progress.PracticePoint += incPracPoint;

                var update = Builders<ClassProgressEntity>.Update;
                var updates = new List<UpdateDefinition<ClassProgressEntity>>();

                if (incCount != 0 || incPoint != 0)
                {
                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;

                    progress.AvgPoint = progress.ExamDone != 0 ? progress.TotalPoint / progress.ExamDone : 0;

                    updates.Add(update.Inc(t => t.ExamDone, incCount)
                        .Inc(t => t.TotalPoint, incPoint)
                        .Set(t => t.AvgPoint, progress.AvgPoint));
                }
                if (incPracCount != 0 || incPracPoint != 0)
                {
                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;

                    progress.PracticeAvgPoint = progress.PracticeDone != 0 ? progress.PracticePoint / progress.PracticeDone : 0;

                    updates.Add(update.Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );
                }
                if (updates.Count > 0)
                    await _classProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID, update.Combine(updates));
            }
        }

        public IEnumerable<StudentRankingViewModel> GetClassResults(string ClassID)
        {
            return _classProgressService.CreateQuery().Find(t => t.ClassID == ClassID).Project(t => new StudentRankingViewModel
            {
                StudentID = t.StudentID,
                AvgPoint = t.AvgPoint,
                ExamDone = t.ExamDone,
                PracticeDone = t.PracticeDone,
                TotalPoint = t.TotalPoint,
                PracticePoint = t.PracticePoint,
                Count = t.Completed,
                LastDate = t.LastDate,
                RankPoint = CalculateRankPoint(t.TotalPoint, t.PracticePoint, t.Completed)
            }).ToEnumerable();
        }

        public IEnumerable<StudentRankingViewModel> GetClassSubjectResults(string ClassSubjectID)
        {
            return _classSubjectProgressService.CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID).Project(t => new StudentRankingViewModel
            {
                StudentID = t.StudentID,
                AvgPoint = t.AvgPoint,
                ExamDone = t.ExamDone,
                PracticeDone = t.PracticeDone,
                TotalPoint = t.TotalPoint,
                PracticePoint = t.PracticePoint,
                Count = t.Completed,
                LastDate = t.LastDate,
                RankPoint = CalculateRankPoint(t.TotalPoint, t.PracticePoint, t.Completed)
            }).ToEnumerable();
        }

        public IEnumerable<StudentRankingViewModel> GetChapterResults(string ChapterID)
        {
            return _chapterProgressService.CreateQuery().Find(t => t.ChapterID == ChapterID)
                .Project(t => new StudentRankingViewModel
                {
                    StudentID = t.StudentID,
                    AvgPoint = t.AvgPoint,
                    PracticeAvgPoint = t.PracticeAvgPoint,
                    ExamDone = t.ExamDone,
                    PracticeDone = t.PracticeDone,
                    TotalPoint = t.TotalPoint,
                    PracticePoint = t.PracticePoint,
                    Count = t.Completed,
                    LastDate = t.LastDate,
                    RankPoint = CalculateRankPoint(t.TotalPoint, t.PracticePoint, t.Completed)
                }).ToEnumerable();
        }

        public double CalculateRankPoint(double examPoint, double practicePoint, double progress)
        {
            return examPoint * 1000 * 1000 + practicePoint * 1000 + progress;
        }
        #endregion

        #region
        public async Task<List<StudentLessonResultViewModel>> GetLessonProgressList(DateTime StartWeek, DateTime EndWeek, StudentEntity student, ClassSubjectEntity classSbj,Boolean isExam = false)
        {

            //TODO: RECHECK !!!!!!!!!!!!!!!!!!
            List<StudentLessonResultViewModel> result = new List<StudentLessonResultViewModel>();
            if (StartWeek > classSbj.EndDate) return result;
            
            //lay danh sach bai hoc trogn tuan
            var activeLessons = _lessonScheduleService.CreateQuery().Find(o => o.ClassSubjectID == classSbj.ID && o.StartDate <= EndWeek && o.EndDate > StartWeek).ToList();
            var activeLessonIds = activeLessons.Select(t => t.LessonID).ToList();

            //danh sach bai luyen tap
            var a = _lessonService.CreateQuery().Find(t => activeLessonIds.Contains(t.ID) && (t.IsPractice || t.TemplateType == LESSON_TEMPLATE.EXAM)).Project(t => t.ID).ToList();
            List<LessonEntity> practices = new List<LessonEntity>();

            if (isExam)
            {
                practices = _lessonService.CreateQuery().Find(x => x.TemplateType == 2 && activeLessonIds.Contains(x.ID)).ToList();
            }
            else
            {
                practices = _lessonService.CreateQuery().Find(x => x.IsPractice == true && activeLessonIds.Contains(x.ID)).ToList();
            }

            foreach (var practice in practices)
            {
                var examresult = _examService.CreateQuery().Find(t => t.StudentID == student.ID && t.LessonID == practice.ID).SortByDescending(t => t.ID).ToList();
                var progress = _lessonProgressService.GetByStudentID_LessonID(student.ID, practice.ID);
                var tried = examresult.Count();
                var maxpoint = tried == 0 ? 0 : examresult.Max(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                var minpoint = tried == 0 ? 0 : examresult.Min(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);
                var avgpoint = tried == 0 ? 0 : examresult.Average(t => t.MaxPoint > 0 ? t.Point * 100 / t.MaxPoint : 0);

                var lastEx = examresult.FirstOrDefault();
                result.Add(new StudentLessonResultViewModel(student)
                {
                    LastTried = lastEx?.Created ?? new DateTime(1900, 1, 1),
                    MaxPoint = maxpoint,
                    MinPoint = minpoint,
                    AvgPoint = avgpoint,
                    TriedCount = tried,
                    LastOpen = progress?.LastDate ?? new DateTime(1900, 1, 1),
                    OpenCount = progress?.TotalLearnt ?? 0,
                    LastPoint = lastEx != null ? (lastEx.MaxPoint > 0 ? lastEx.Point * 100 / lastEx.MaxPoint : 0) : 0,
                    IsCompleted = lastEx != null && lastEx.Status,
                    ListExam = examresult.Select(t => new ExamDetailCompactView(t)).ToList(),
                    LessonName = practice.Title,
                    LessonID = practice.ID,
                    ClassSubjectID = practice.ClassSubjectID
                });
            }
            return result;
        }
        #endregion

    }
}
