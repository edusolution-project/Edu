using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "SysTemplateDetails")]
    public class SysTemplateDetailsEntity : EntityBase
    {
        public string ParrentID { get; set; } //ID layout - vswMain  LayoutID dynamic
        public string LayoutName { get; set; } // name - Logo(ĐK : Quảng cáo/liên kết)
        public int TemplateID { get; set; } // templateid 1,2,3
        public string PartialID { get; set; } // ID layout - vswMain / vswLogo
        public string PartialView { get; set; } // view / _Nav
        public string CModule { get; set; } // chucws nang /MVCBase.ClientControllers.CAdvController
        public string TypeView { get; set; } // CAdv ..
        public bool IsBody { get; set; } = false; //laf body khongo 
        public bool IsDynamic { get; set; } = false; // cos phai dynamic khong
        public int Order { get; set; } // sap xep trong isDynamic

        public ICollection<SysPropertyEntity> GetProperties()
        {
            SysPropertyService propertyService = new SysPropertyService();
            var data = propertyService.CreateQuery().Find(o => o.TemplateDetailID == ID && o.PartialID == PartialID).ToList();
            return data ?? null;
        }
    }
    public class SysTemplateDetailsService : ServiceBase<SysTemplateDetailsEntity>
    {
        #region Private
        public SysTemplateDetailsService() : base("SysTemplateDetails")
        {

        }
        #endregion
        public List<SysTemplateDetailsEntity> GetItemParrentID(int ID)
        {
            return CreateQuery().Find(o => o.TemplateID == ID).ToList();
        }
        public SysTemplateDetailsEntity GetItemStaticByID(int templateID,string layoutID)
        {
            var data = CreateQuery().SingleOrDefault(o => o.IsDynamic == false && o.TemplateID == templateID && o.PartialID == layoutID);
            return data;
        }
        public List<SysTemplateDetailsEntity> GetListItemDynamicByID(int templateID, string layoutID)
        {
            var data = CreateQuery().Find(o => o.IsDynamic == true && o.TemplateID == templateID && o.ParrentID == layoutID).ToList();
            return data;
        }
        public SysTemplateDetailsEntity GetItemDynamicByID(int templateID, string partialID, string layoutParrent)
        {
            var data = CreateQuery().SelectFirst(o => o.IsDynamic == true && o.TemplateID == templateID && o.PartialID == partialID && o.ParrentID == layoutParrent);
            return data;
        }

        public SysTemplateDetailsEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        
        public List<SysTemplateDetailsEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(SysTemplateDetailsEntity item)
        {
            if(item.ID == 0)
            {
                CreateQuery().Add(item);
                return CreateQuery().Complete();
            }
            else
            {
                var current = GetItemByID(item.ID);
                if(current != null)
                {
                    CreateQuery().Update(current, item);
                    return CreateQuery().Complete();
                }
                else
                {
                    return -1;
                }
            }
        }
        public async Task<int> SaveAsync(SysTemplateDetailsEntity item)
        {
            if (item.ID == 0)
            {
                CreateQuery().Add(item);
                return await CreateQuery().CompleteAsync();
            }
            else
            {
                var current = GetItemByID(item.ID);
                if (current != null)
                {
                    CreateQuery().Update(current, item);
                    return await CreateQuery().CompleteAsync();
                }
                else
                {
                    return -1;
                }
            }
        }
        public int Save(List<SysTemplateDetailsEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
