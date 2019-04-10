using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "SysPages")]
    public class SysPageEntity : EntityBase
    {
        public int ParentID { get; set; }
        public string CModule { get; set; }
        public string CMethod { get; set; } // name view
        public int LangID { get; set; }
        public int MenuID { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Customer { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public bool Activity { get; set; }
        public int Order { get; set; }
        public int TemplateID { get; set; }
        public DateTime Created { get; set; }
    }
    public class SysPageService : ServiceBase<SysPageEntity>
    {
        #region Private
        public SysPageService() : base("SysPages")
        {

        }
        #endregion

        
        public SysPageEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }

        public SysPageEntity GetItemByCode(string code)
        {
            return CreateQuery().SingleOrDefault(o => o.Code == code);
        }

        public SysPageEntity GetItemByCtrlandAct(string ctrlName, string actName)
        {
            
            return CreateQuery().SingleOrDefault(o => o.CModule == ctrlName && o.CMethod == actName);
        }

        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public List<SysPageEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(SysPageEntity item)
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
        public int Save(List<SysPageEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
