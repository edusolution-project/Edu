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
    public class ModLessonController : ControllerBase
    {
        private readonly ModLessonService _lessonService;
        public ModLessonController(ModLessonService lessonService)
        {
            _lessonService = lessonService;
        }
        [Obsolete]
        [HttpPost]
        public Task<BaseResponse<ModLessonEntity>> GetList([FromBody] SeachForm seachForm)
        {
            return _lessonService.getList(seachForm);
        }
        [HttpPost]
        public Task<ModLessonEntity> GetDetail([FromBody] ModLessonEntity item)
        {
            var current = _lessonService.GetByID(item.ID);
            return current == null ? null : Task.FromResult(current);
        }
        [HttpPost]
        public async Task<ModLessonEntity> Create([FromBody] ModLessonEntity item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                item.Created = DateTime.Now;
                await _lessonService.Collection.InsertOneAsync(item);
                return item;
            }
            else
            {
                var current = _lessonService.GetByID(item.ID);
                if (current != null)
                {
                    current.Updated = DateTime.Now;
                    await _lessonService.Collection.ReplaceOneAsync(o => o.ID == item.ID, item);
                    return item;
                }
                else
                {
                    item.Created = DateTime.Now;
                    await _lessonService.Collection.InsertOneAsync(item);
                    return item;
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ModLessonEntity item)
        {
            await _lessonService.RemoveAsync(item.ID);
            return NoContent();
        }
    }
}