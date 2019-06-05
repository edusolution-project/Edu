using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModTags")]
    public class ModTagEntity : EntityBase
    {
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int RootID { get; set; }
        public int NewID { get; set; }
        public int MenuID { get; set; }
        public int Order { get; set; }
        public bool Activity { get; set; } = false;
        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class ModTagService : ServiceBase<ModTagEntity>
    {
        #region Private
        public ModTagService() : base("ModTags")
        {

        }
        #endregion

        
        public ModTagEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
       
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
       
        public List<ModTagEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModTagEntity item)
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
        public int Save(List<ModTagEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
