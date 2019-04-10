using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExampleProperties")]
    public class ModExamplePropertyEntity : EntityBase
    {
        public int QuetionsID { get; set; }
        public string Value { get; set; }
        public string Media { get; set; }// dạng audio mp3
        public bool IsAnswer { get; set; } = false;
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
    }
    public class ModExamplePropertieservice : ServiceBase<ModExamplePropertyEntity>
    {
        #region Private
        public ModExamplePropertieservice() : base("ModExampleProperties")
        {

        }
        #endregion

        public ModExamplePropertyEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public List<ModExamplePropertyEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModExamplePropertyEntity item)
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
        public int Save(List<ModExamplePropertyEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
