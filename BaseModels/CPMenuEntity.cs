using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "CPMenus")]
    public class CPMenuEntity : EntityBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int ParentID { get; set; }
        public string Summary { get; set; }
        public int LangID { get; set; }
        public string Files { get; set; }
        public string Content { get; set; }
        public bool Activity { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class CPMenuService : ServiceBase<CPMenuEntity>
    {
        #region Private
        public CPMenuService() : base("CPMenus")
        {

        }
        #endregion
        public List<CPMenuEntity> GetItemByType(string type,int langID)
        {
            var data = CreateQuery().Find(o => o.Type == type && o.LangID == langID).ToList();
            return data;
        }
        public List<CPMenuEntity> GetItemByType(string type)
        {
            var data = CreateQuery().Find(o => o.Type == type).ToList();
            return data;
        }
        public CPMenuEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public CPMenuEntity GetRoot(string type)
        {
            return CreateQuery().SingleOrDefault(o => o.Activity == true && o.ParentID == 0 && o.Type == type);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        
        public List<CPMenuEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(CPMenuEntity item)
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
        public int Save(List<CPMenuEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(CPMenuEntity item)
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
    }
}
