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
    public class ModLessonPartController : ControllerBase
    {
        private readonly ModLessonPartService _lessonPartService;
        public ModLessonPartController(ModLessonPartService lessonPartService)
        {
            _lessonPartService = lessonPartService;
        }
        [Obsolete]
        [HttpPost]
        public Task<BaseResponse<ModLessonPartEntity>> GetList([FromBody] SeachForm seachForm)
        {
            return _lessonPartService.getList(seachForm);
        }
        [HttpPost]
        public Task<ModLessonPartEntity> GetDetail([FromBody] ModLessonPartEntity item)
        {
            var current = _lessonPartService.GetByID(item.ID);
            return current == null ? null : Task.FromResult(current);
        }
        [HttpPost]
        public async Task<ModLessonPartEntity> Create([FromBody] ModLessonPartEntity item)
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
        public async Task<IActionResult> Delete([FromBody]ModLessonPartEntity item)
        {
            await _lessonPartService.RemoveAsync(item.ID);
            return NoContent();
        }
    }
}