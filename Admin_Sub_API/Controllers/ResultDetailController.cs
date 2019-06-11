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
    public class ResultDetailController : ControllerBase
    {
        public readonly ModResultDetailService _resultDetailService;
        public ResultDetailController(ModResultDetailService resultDetailService)
        {
            _resultDetailService = resultDetailService;
        }
        [HttpPost]
        [Obsolete]
        public Task<BaseResponse<ModResultDetailEntity>> GetListByResult([FromBody] SeachForm seachForm,string ResultID)
        {
            return _resultDetailService.getListByResult(seachForm, ResultID);
        }

        [Obsolete]
        [HttpPost]
        public Task<BaseResponse<ModResultDetailEntity>> GetList([FromBody] SeachForm seachForm)
        {
            return _resultDetailService.getList(seachForm);
        }
        [HttpPost]
        public Task<ModResultDetailEntity> GetDetail([FromBody] ModResultDetailEntity item)
        {
            var current = _resultDetailService.GetByID(item.ID);
            return current == null ? null : Task.FromResult(current);
        }
        [HttpPost]
        public async Task<ModResultDetailEntity> Create([FromBody] ModResultDetailEntity item)
        {
            if (string.IsNullOrEmpty(item.ID))
            {
                item.Created = DateTime.Now;
                await _resultDetailService.Collection.InsertOneAsync(item);
                return item;
            }
            else
            {
                var current = _resultDetailService.GetByID(item.ID);
                if(current != null)
                {
                    current.Update = DateTime.Now;
                    current.ResultID = item.ResultID;
                    await _resultDetailService.Collection.ReplaceOneAsync(o => o.ID == item.ID, item);
                    return item;
                }
                else
                {
                    item.Created = DateTime.Now;
                    await _resultDetailService.Collection.InsertOneAsync(item);
                    return item;
                }
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody]ModResultDetailEntity item)
        {
            await _resultDetailService.RemoveAsync(item.ID);
            return NoContent();
        }
        //get list
        //get Details
        //post list
        //post single
        //edit
    }
}