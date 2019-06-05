using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;

namespace BaseModels
{
    [Table(name: "SysTemplates")]
    public class SysTemplateEntity : EntityBase
    {
        public string Name { get; set; }
        public int LangID { get; set; }
        public string File { get; set; } // ten layout
        public string Html { get; set; }
    }
    public class SysTemplateService : ServiceBase<SysTemplateEntity>
    {
        #region Private
        public SysTemplateService() : base("SysTemplates")
        {

        }
        #endregion
        
        public SysTemplateEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        
        public List<SysTemplateEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(SysTemplateEntity item)
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
        public int Save(List<SysTemplateEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
