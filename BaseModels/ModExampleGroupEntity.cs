using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExampleGroups")]
    public class ModExampleGroupEntity : EntityBase
    {
        public int ExampleID { get; set; }
        public int ExampleQuetionID { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class ModExampleGroupService : ServiceBase<ModExampleGroupEntity>
    {
        #region Private
        public ModExampleGroupService() : base("ModExampleGroups")
        {

        }
        #endregion

        public ModExampleGroupEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
       
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
       
        public List<ModExampleGroupEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModExampleGroupEntity item)
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
        public int Save(List<ModExampleGroupEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
