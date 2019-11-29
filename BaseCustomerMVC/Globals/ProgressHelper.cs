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
        private readonly ClassProgressService _classProgressService;
        private readonly LessonProgressService _lessonProgressService;
        private readonly ChapterProgressService _chapterProgressService;


        public ProgressHelper(
            LearningHistoryService learningHistoryService,
            ClassProgressService classProgressService,
            LessonProgressService lessonProgressService,
            ChapterProgressService chapterProgressService
        )
        {
            _learningHistoryService = learningHistoryService;
            _classProgressService = classProgressService;
            _chapterProgressService = chapterProgressService;
            _lessonProgressService = lessonProgressService;
        }

        public async Task CreateHist(LearningHistoryEntity item)
        {
            var historycount = _learningHistoryService.CountHistory(item);
            item.Time = DateTime.Now;
            item.ViewCount = (int)historycount;
            _learningHistoryService.CreateOrUpdate(item);
            await _classProgressService.UpdateLastLearn(item);
            await _lessonProgressService.UpdateLastLearn(item);
            await _chapterProgressService.UpdateLastLearn(item);
        }

    }
}
