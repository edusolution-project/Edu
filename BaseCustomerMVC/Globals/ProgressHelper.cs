using BaseCustomerEntity.Database;
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
        private readonly LearningHistoryService _learningHistoryService;
        
        
        private readonly LessonProgressService _lessonProgressService;
        private readonly ChapterProgressService _chapterProgressService;
        private readonly ClassSubjectProgressService _classSubjectProgressService;
        private readonly ClassProgressService _classProgressService;

        public ProgressHelper(
            LearningHistoryService learningHistoryService,
            
            LessonProgressService lessonProgressService,
            ChapterProgressService chapterProgressService,
            ClassSubjectProgressService classSubjectProgressService,
            ClassProgressService classProgressService
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

            var lessonProgress = _lessonProgressService.GetByClassSubjectID_StudentID_LessonID(item.ClassSubjectID, item.StudentID, item.LessonID);

            await _chapterProgressService.UpdateLastLearn(lessonProgress);
            await _classSubjectProgressService.UpdateLastLearn(lessonProgress);
            await _classProgressService.UpdateLastLearn(lessonProgress);
        }

    }
}
