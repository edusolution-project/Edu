using CoreMongoDB.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BaseMongoDB.Database
{
    public class SysTemplateDetailEntity : EntityBase
    {
        public string ParrentID { get; set; } //ID layout - vswMain  LayoutID dynamic
        public string LayoutName { get; set; } // name - Logo(ĐK : Quảng cáo/liên kết)
        public string TemplateID { get; set; } // templateid 1,2,3
        public string PartialID { get; set; } // ID layout - vswMain / vswLogo
        public string PartialView { get; set; } // view / _Nav
        public string CModule { get; set; } // chucws nang /MVCBase.ClientControllers.CAdvController
        public string TypeView { get; set; } // CAdv ..
        public bool IsBody { get; set; } = false; //laf body khongo 
        public bool IsDynamic { get; set; } = false; // cos phai dynamic khong
        public int Order { get; set; } // sap xep trong isDynamic
    }
    public class SysTemplateDetailService : ServiceBase<SysTemplateDetailEntity>
    {
        public SysTemplateDetailService(IConfiguration config) : base(config, "SysTemplateDetails")
        {

        }

        public SysTemplateDetailService(IConfiguration config, string tableName) : base(config, tableName)
        {
        }

        public List<SysTemplateDetailEntity> GetItemParrentID(string ID)
        {
            return CreateQuery().Find(o => o.TemplateID == ID)?.ToList();
        }
        public SysTemplateDetailEntity GetItemStaticByID(string templateID, string layoutID)
        {
            var data = CreateQuery().Find(o => o.IsDynamic == false && o.TemplateID == templateID && o.PartialID == layoutID)?.SingleOrDefault();
            return data;
        }
        public List<SysTemplateDetailEntity> GetListItemDynamicByID(string templateID, string layoutID)
        {
            var data = CreateQuery().Find(o => o.IsDynamic == true && o.TemplateID == templateID && o.ParrentID == layoutID)?.ToList();
            return data;
        }
        public SysTemplateDetailEntity GetItemDynamicByID(string templateID, string partialID, string layoutParrent)
        {
            var data = CreateQuery().Find(o => o.IsDynamic == true && o.TemplateID == templateID && o.PartialID == partialID && o.ParrentID == layoutParrent)?.SingleOrDefault();
            return data;
        }
    }
}
