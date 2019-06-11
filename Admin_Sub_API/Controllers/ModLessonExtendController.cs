using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseMongoDB.Database;
using Business.Dto.Form;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
namespace Admin_Sub_API.Controllers
{
    [ApiController]
    public class ModLessonExtendController : ControllerBase
    {
        private readonly ModLessonExtendService _lessonPartService;
        public ModLessonExtendController(ModLessonExtendService lessonPartService)
        {
            _lessonPartService = lessonPartService;
        }
        [Obsolete]
        [HttpPost]
        public Task<BaseResponse<ModLessonExtendEntity>> GetList([FromBody] SeachForm seachForm)
        {
            return _lessonPartService.getList(seachForm);
        }
        [HttpPost]
        public Task<ModLessonExtendEntity> GetDetail([FromBody] ModLessonExtendEntity item)
        {
            var current = _lessonPartService.GetByID(item.ID);
            return current == null ? null : Task.FromResult(current);
        }
        [HttpPost]
        public async Task<ModLessonExtendEntity> Create([FromBody] ModLessonExtendEntity item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                item.Created = DateTime.Now;
                await _lessonPartService.Collection.InsertOneAsync(item);
                return item;
            }
            else
            {
                var current = _lessonPartService.GetByID(item.ID);
                if (current != null)
                {
                    current.Updated = DateTime.Now;
                    await _lessonPartService.Collection.ReplaceOneAsync(o => o.ID == item.ID, item);
                    return item;
                }
                else
                {
                    item.Created = DateTime.Now;
                    await _lessonPartService.Collection.InsertOneAsync(item);
                    return item;
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ModLessonExtendEntity item)
        {
            await _lessonPartService.RemoveAsync(item.ID);
            return NoContent();
        }
    }
}