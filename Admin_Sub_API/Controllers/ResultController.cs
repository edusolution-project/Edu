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
    public class ResultController : ControllerBase
    {
        private readonly ModResultService _resultService;
        public ResultController(ModResultService resultService)
        {
            _resultService = resultService;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public Task<BaseResponse<ModResultEntity>> GetListByStudent([FromBody]SeachForm seachForm)
        {
            return _resultService.getListByStudentID(seachForm);
        }
        [HttpPost]
        [Obsolete]
        public Task<BaseResponse<ModResultEntity>> GetList([FromBody]SeachForm seachForm)
        {
            return _resultService.getList(seachForm);
        }
        [HttpPost]
        [Obsolete]
        public Task<ModResultEntity> StartDoing([FromBody]ModResultEntity item)
        {
            var itemOld = _resultService.CreateQuery().Find(o => o.UserDoID == item.UserDoID && o.LessonID == item.LessonID);
            if(itemOld == null)
            {
                item.Created = DateTime.Now;
                item.Status = 0;
                item.NumberDo = 1;
                _resultService.CreateQuery().InsertOne(item);
                return Task.FromResult(item);
            }
            else
            {
                item.Created = DateTime.Now;
                item.Status = 0;
                item.NumberDo = itemOld.Count();
                _resultService.CreateQuery().InsertOne(item);
                return Task.FromResult(item);

            }
        }
        [HttpPost]
        [Obsolete]
        public Task<ModResultEntity> Finished([FromBody]ModResultEntity item)
        {
            var itemOld = _resultService.CreateQuery().Find(o=>o.ID == item.ID).SingleOrDefault();
            if (itemOld == null) return null;
            else
            {
                itemOld.Update = DateTime.Now;
                itemOld.Status = 1;
                _resultService.CreateQuery().ReplaceOne(o => o.ID == item.ID, itemOld);
                return Task.FromResult(itemOld);
            }
        }

        //getlist
        //getdetails
        //postView
        //update status
    }
}