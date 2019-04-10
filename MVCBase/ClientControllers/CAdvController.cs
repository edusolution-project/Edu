using BaseModels;
using Microsoft.AspNetCore.Mvc;
using MVCBase.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVCBase.ClientControllers
{
    [MenuControl( 
        Name = "ĐK : Quảng cáo/liên kết",
        CModule ="CAdv",
        IsControl = true,
        IShow = false,
        Order =21
     )]
    public class CAdvController : ClientController
    {

        [Property("Tiêu đề")]
        public string Title = "";
        [Property("Chuyên mục","type|CAdv")]
        public int MenuID = 0;
        private ModCAdvService _advService;
        public CAdvController(string title, int menuID)
        {
            Title = title;
            MenuID = menuID;
            _advService = new ModCAdvService();
        }

        public dynamic OnLoad()
        {
            string keyCache = "_CAdv_"+MenuID+"_";
            var cachevalue = CacheExtends.GetDataFromCache<dynamic>(keyCache);
            if(cachevalue != null && cachevalue.Data != null)
            {
                return cachevalue;
            }
            else
            {
                ViewBag.Title = Title;
                var data = ChildNodes.GetChildMenuByRoot(MenuID);
                var cache = _advService.CreateQuery().Find(o => o.Activity == true && data.IndexOf(o.MenuID) > -1).ToList();
                ViewBag.Data = cache;
                CacheExtends.SetObjectFromCache(keyCache, 120, ViewBag);
                return ViewBag;
            }
        }
    }
}
