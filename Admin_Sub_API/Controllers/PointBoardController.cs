using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseMongoDB.Database;
using Microsoft.AspNetCore.Mvc;

namespace Admin_Sub_API.Controllers
{
    public class PointBoardController : ControllerBase
    {
        private readonly ModPointBoardService _pointBoardService;
        
        public PointBoardController(ModPointBoardService pointBoardService)
        {
            _pointBoardService = pointBoardService;
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