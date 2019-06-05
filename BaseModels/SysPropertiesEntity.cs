using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;

namespace BaseModels
{
    [Table(name: "SysProperties")]
    public class SysPropertyEntity : EntityBase
    {
        public int TemplateDetailID { get; set; }  // id templatedetails
        public string Name { get; set; } // key
        public string Value { get; set; } // value
        public string PartialID { get; set; } // vswLogo
    }
    public class SysPropertyService : ServiceBase<SysPropertyEntity>
    {
        #region Private
        public SysPropertyService() : base("SysProperties")
        {

        }
        #endregion
        
        public SysPropertyEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }

        public List<SysPropertyEntity> GetItemByParentID(int ID)
        {
            return CreateQuery().Find(o => o.TemplateDetailID == ID).ToList();
        }

        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        
        public List<SysPropertyEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(SysPropertyEntity item)
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
        public int Save(List<SysPropertyEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
