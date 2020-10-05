using BaseCustomerEntity.Database;
using BaseCustomerMVC.Models;
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

        public ProgressHelper(
            ClassProgressService classProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ChapterProgressService chapterProgressService,
            LessonProgressService lessonProgressService,
            LearningHistoryService learningHistoryService
        )
        {
            _learningHistoryService = learningHistoryService;
            _lessonProgressService = lessonProgressService;
            _chapterProgressService = chapterProgressService;
            _classSubjectProgressService = classSubjectProgressService;
            _classProgressService = classProgressService;
        }

        public async Task CreateHist(LearningHistoryEntity item)
        {
            var historycount = _learningHistoryService.CountHistory(item);
            item.Time = DateTime.Now;
            item.ViewCount = (int)historycount;
            _learningHistoryService.CreateOrUpdate(item);

            await _lessonProgressService.UpdateLastLearn(item);
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



        public List<StudentRankingViewModel> GetClassResults(string ClassID)
        {
            return _classProgressService.CreateQuery().Find(t => t.ClassID == ClassID).Project(t => new StudentRankingViewModel
            {
                StudentID = t.StudentID,
                AvgPoint = t.AvgPoint,
                ExamDone = t.ExamDone,
                TotalPoint = t.TotalPoint,
                PracticePoint = t.PracticePoint,
                Count = t.Completed,
                LastDate = t.LastDate,
                RankPoint = CalculateRankPoint(t.TotalPoint, t.PracticePoint, t.Completed)
            }).ToEnumerable().ToList();
        }

        public IEnumerable<StudentRankingViewModel> GetClassSubjectResults(string ClassSubjectID)
        {
            return _classSubjectProgressService.CreateQuery().Find(t => t.ClassSubjectID == ClassSubjectID).Project(t => new StudentRankingViewModel
            {
                StudentID = t.StudentID,
                AvgPoint = t.AvgPoint,
                ExamDone = t.ExamDone,
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
