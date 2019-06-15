using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseMongoDB.Database;
using Business.Dto.Form;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Admin_Sub_API.Controllers
{
    public class PointBoardController : ControllerBase
    {
        private readonly ModPointBoardService _pointBoardService;
        
        public PointBoardController(ModPointBoardService pointBoardService)
        {
            _pointBoardService = pointBoardService;
        }

        [Obsolete]
        [HttpPost]
        public Task<BaseResponse<ModPointBoardEntity>> GetList([FromBody] SeachForm seachForm)
        {
            return _pointBoardService.getList(seachForm);
        }
        [HttpPost]
        public Task<ModPointBoardEntity> GetDetail([FromBody] ModPointBoardEntity item)
        {
            var current = _pointBoardService.GetByID(item.ID);
            return current == null ? null : Task.FromResult(current);
        }
        [HttpPost]
        public async Task<ModPointBoardEntity> Create([FromBody] ModPointBoardEntity item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                item.Created = DateTime.Now;
                await _pointBoardService.Collection.InsertOneAsync(item);
                return item;
            }
            else
            {
                var current = _pointBoardService.GetByID(item.ID);
                if (current != null)
                {
                    current.Updated = DateTime.Now;
                    await _pointBoardService.Collection.ReplaceOneAsync(o => o.ID == item.ID, item);
                    return item;
                }
                else
                {
                    item.Created = DateTime.Now;
                    await _pointBoardService.Collection.InsertOneAsync(item);
                    return item;
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ModLessonPartQuestionEntity item)
        {
            await _pointBoardService.RemoveAsync(item.ID);
            return NoContent();
        }
        //getList
        //getListByStudent
        //getListByTeacher
        //getDetails
        //Create
        //private
        ///====> caculator point by listResultDetails from Result status 1;

    }
}