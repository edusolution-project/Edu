using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExampleQuestions")]
    public class ModExampleQuestionEntity : EntityBase
    {
        public string Question { get; set; }
        public string Code { get; set; }
        public int Type { get; set; } // 0 - 1 - 2 - 3
        public bool IsMutil { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
    }
    public class ModExampleQuestionService : ServiceBase<ModExampleQuestionEntity>
    {
        #region Private
        public ModExampleQuestionService() : base("ModExampleQuestions")
        {

        }
        #endregion

        public ModExampleQuestionEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public List<ModExampleQuestionEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModExampleQuestionEntity item)
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
        public int Save(List<ModExampleQuestionEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
