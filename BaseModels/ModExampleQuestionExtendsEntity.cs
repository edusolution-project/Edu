using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExampleQuestionExtends")]
    public class ModExampleQuestionExtendEntity : EntityBase
    {
        public int QuestionID { get; set; }
        public int Type { get; set; }// 0 - 1 - 2 - 3 (img - video - audio)
        public string Media { get; set; } 
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    /// <summary>
    /// Bảng lưu trữ thông tin media 
    /// </summary>
    public class ModExampleQuestionExtendService : ServiceBase<ModExampleQuestionExtendEntity>
    {
        #region Private
        public ModExampleQuestionExtendService() : base("ModExampleQuestionExtends")
        {

        }
        #endregion

        public ModExampleQuestionExtendEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public List<ModExampleQuestionExtendEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModExampleQuestionExtendEntity item)
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
        public int Save(List<ModExampleQuestionExtendEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
