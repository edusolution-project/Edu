using BaseCustomerEntity.Database;
using Core_v2.Globals;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCustomerMVC.Globals
{
    public class ClassHelper
    {
        private readonly ChapterService _chapterService;
        private readonly ClassSubjectService _classSubjectService;
        private readonly ClassService _classService;



        public ClassHelper(
            ChapterService chapterService,
            ClassSubjectService classSubjectService,
            ClassService classService
        )
        {
            _chapterService = chapterService;
            _classSubjectService = classSubjectService;
            _classService = classService;
        }

        internal async Task ChangeLessonPracticeState(LessonEntity lesson)
        {
            if (lesson.ChapterID != "0")
                await IncreaseChapterCounter(lesson.ChapterID, 0, 0, lesson.IsPractice ? 1 : -1);
            else
                await IncreaseClassSubjectCounter(lesson.ClassSubjectID, 0, 0, lesson.IsPractice ? 1 : -1);
        }

        public async Task IncreaseChapterCounter(string ID, long lesInc, long examInc, long pracInc, List<string> listid = null)//prevent circular ref
        {
            var r = await _chapterService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ChapterEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
            if (r.ModifiedCount > 0)
            {
                if (listid == null)
                    listid = new List<string> { ID };
                else
                    listid.Add(ID);
                var current = _chapterService.GetItemByID(ID);
                if (current != null)
                {
                    if (!string.IsNullOrEmpty(current.ParentID) && (current.ParentID != "0"))
                    {
                        if (listid.IndexOf(current.ParentID) < 0)
                            await IncreaseChapterCounter(current.ParentID, lesInc, examInc, pracInc, listid);
                    }
                    else
                        await IncreaseClassSubjectCounter(current.ClassSubjectID, lesInc, examInc, pracInc);
                }
            }
        }

        public async Task IncreaseClassSubjectCounter(string ID, long lesInc, long examInc, long pracInc)
        {
            var r = await _classSubjectService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ClassSubjectEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
            if (r.ModifiedCount > 0)
            {
                var current = _classSubjectService.GetItemByID(ID);
                if (current != null)
                {
                    await IncreaseClassCounter(current.ClassID, lesInc, examInc, pracInc);
                }
            }
        }

        public async Task IncreaseClassCounter(string ID, long lesInc, long examInc, long pracInc)
        {
            var r = await _classService.CreateQuery().UpdateOneAsync(t => t.ID == ID, new UpdateDefinitionBuilder<ClassEntity>()
                .Inc(t => t.TotalLessons, lesInc)
                .Inc(t => t.TotalExams, examInc)
                .Inc(t => t.TotalPractices, pracInc));
        }

    }
}
