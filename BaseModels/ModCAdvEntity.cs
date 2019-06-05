using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModCAdvs")]
    public class ModCAdvEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string File { get; set; }
        public string Summary { get; set; }
        public string URL { get; set; }
        public int MenuID { get; set; }
        public int Order { get; set; }
        public bool Activity { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class ModCAdvService : ServiceBase<ModCAdvEntity>
    {
        #region Private
        public ModCAdvService() : base("ModCAdvs")
        {

        }
        #endregion
        public ModCAdvEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public bool IsExistCode(string code)
        {
            return CreateQuery().SingleOrDefault(o => o.Code == code) != null;
        }
        public List<ModCAdvEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModCAdvEntity item)
        {
            if(item.ID == 0)
            {
                if (IsExistCode(item.Code))
                {
                    item.Code = item.Code + Guid.NewGuid().ToString();
                }
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
        public int Save(List<ModCAdvEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(ModCAdvEntity item)
        {
            if (item.ID == 0)
            {
                if (IsExistCode(item.Code))
                {
                    item.Code = item.Code + Guid.NewGuid().ToString();
                }
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
