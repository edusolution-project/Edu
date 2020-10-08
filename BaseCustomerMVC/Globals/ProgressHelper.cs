using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
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

        public ProgressHelper(
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ChapterProgressService chapterProgressService,
            LessonProgressService lessonProgressService,
            LearningHistoryService learningHistoryService,

            LessonService lessonService,
            ChapterService chapterService,
            ClassSubjectService classSubjectService,
            ClassService classService
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
        }

        public async Task CreateHist(LearningHistoryEntity item)
        {
            var historycount = _learningHistoryService.CountHistory(item);
            item.Time = DateTime.Now;
            item.ViewCount = (int)historycount;
            _learningHistoryService.CreateOrUpdate(item);

            await _lessonProgressService.UpdateLearn(item);
            var lessonProgress = _lessonProgressService.GetByStudentID_LessonID(item.StudentID, item.LessonID);

            await _chapterProgressService.UpdateLastLearn(lessonProgress);
            await _classSubjectProgressService.UpdateLastLearn(lessonProgress);
            await _classProgressService.UpdateLastLearn(lessonProgress);
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
            return Task.CompletedTask;
        }

        public async Task RemoveClassSubjectHistory(string ClassSubjectID)
        {
            await _learningHistoryService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            var subjectProgresses = _classSubjectProgressService.GetListOfCurrentSubject(ClassSubjectID);
            if (subjectProgresses != null)
                foreach (var progress in subjectProgresses)
                    await _classProgressService.DecreaseClassSubject(progress);//remove subject progress from class progress
            await _classSubjectProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            await _chapterProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
            await _lessonProgressService.CreateQuery().DeleteManyAsync(t => t.ClassSubjectID == ClassSubjectID);
        }

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

        public async Task<LessonProgressEntity> UpdateLessonPoint(ExamEntity item)
        {
            return await _lessonProgressService.UpdatePoint(item);
        }

        public async Task UpdateChapterPoint(LessonProgressEntity item, double pointchange = 0)
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
            if (progress == null)
            {
                return;
            }
            else
            {
                var point = (pointchange > 0 ? pointchange : item.PointChange) * item.Multiple;

                var incPoint = 0.0;
                long incCount = 0;
                var incPracPoint = 0.0;
                long incPracCount = 0;

                if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                {
                    incPoint = point;

                    if (item.Tried == 1 || progress.ExamDone == 0)//new
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
                    if (item.Tried == 1 || progress.PracticeDone == 0)//new
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
            }
        }

        private async Task UpdateParentChapPoint(string ChapterID, string StudentID, double incPoint, long incCount, double incPracPoint, long incPracCount)
        {
            var chapter = _chapterService.GetItemByID(ChapterID);
            if (chapter == null)
            {
                return;
            }
            var progress = _chapterProgressService.GetItemByChapterID(chapter.ID, StudentID);
            if (progress == null)
            {
                return;
            }
            else
            {
                if (incCount != 0 || incPoint != 0)
                {
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

                if (incPracCount != 0 || incPracPoint != 0)
                {
                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;

                    progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;

                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassSubjectProgressEntity>.Update
                        .Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );
                }

                if (chapter.ParentID != "0")
                    await UpdateParentChapPoint(chapter.ParentID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);
                else
                    await UpdateClassSubjectPoint(chapter.ClassSubjectID, progress.StudentID, incPoint, incCount, incPracPoint, incPracCount);

            }
        }

        public async Task UpdateClassSubjectPoint(LessonProgressEntity item, double pointchange = 0)
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
                var point = (pointchange > 0 ? pointchange : item.PointChange) * item.Multiple;

                var incPoint = 0.0;
                long incCount = 0;
                var incPracPoint = 0.0;
                long incPracCount = 0;

                if (lesson.TemplateType == LESSON_TEMPLATE.EXAM)
                {
                    incPoint = point;

                    if (item.Tried == 1 || progress.ExamDone == 0)//new
                        incCount = (long)item.Multiple;

                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;
                    progress.AvgPoint = progress.TotalPoint / progress.ExamDone;

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
                    if (item.Tried == 1 || progress.PracticeDone == 0)//new
                        incPracCount = (long)item.Multiple;

                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;
                    progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;

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
                if (incCount != 0 || incPoint != 0)
                {
                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;
                    progress.AvgPoint = progress.TotalPoint / progress.ExamDone;

                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassSubjectProgressEntity>.Update
                        .Inc(t => t.ExamDone, incCount)
                        .Inc(t => t.TotalPoint, incPoint)
                        .Set(t => t.AvgPoint, progress.AvgPoint)
                        );
                }

                if (incPracCount != 0 || incPracPoint != 0)
                {
                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;

                    progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;

                    await _classSubjectProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassSubjectProgressEntity>.Update
                        .Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );
                }

                await UpdateClassPoint(classSbj.ClassID, StudentID, incPoint, incCount, incPracPoint, incPracCount);
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

                if (incCount != 0 || incPoint != 0)
                {
                    progress.ExamDone += incCount;
                    progress.TotalPoint += incPoint;

                    progress.AvgPoint = progress.TotalPoint / progress.ExamDone;

                    await _classProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassProgressEntity>.Update
                        .Inc(t => t.ExamDone, incCount)
                        .Inc(t => t.TotalPoint, incCount)
                        .Set(t => t.AvgPoint, progress.AvgPoint)
                        );
                }
                if (incPracCount != 0 || incPracPoint != 0)
                {
                    progress.PracticeDone += incPracCount;
                    progress.PracticePoint += incPracPoint;

                    progress.PracticeAvgPoint = progress.PracticePoint / progress.PracticeDone;

                    await _classProgressService.CreateQuery().UpdateOneAsync(t => t.ID == progress.ID,
                        Builders<ClassProgressEntity>.Update
                        .Inc(t => t.PracticeDone, incPracCount)
                        .Inc(t => t.PracticePoint, incPracPoint)
                        .Set(t => t.PracticeAvgPoint, progress.PracticeAvgPoint)
                        );
                }
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


    }
}
