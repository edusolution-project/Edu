using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModNews")]
    public class ModNewEntity : EntityBase
    {
        public string PageDescription { get; set; }
        public string PageKeywords { get; set; }
        public string PageTitle { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string File { get; set; }
        public int LangID { get; set; }
        public int State { get; set; }
        public int View { get; set; }
        public int MenuID { get; set; }
        public int Order { get; set; }
        public bool Activity { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;
        [Column(TypeName = "DateTime2")]
        public DateTime Updated { get; set; } = DateTime.Now;
    }
    public class ModNewService : ServiceBase<ModNewEntity>
    {
        #region Private
        public ModNewService() : base("ModNews")
        {

        }
        #endregion

        public ModNewEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public ModNewEntity GetItemByCode(string code)
        {
            return CreateQuery().SingleOrDefault(o => o.Code == code);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public bool IsExistByCode(string code)
        {
            return GetItemByCode(code) != null;
        }
        public List<ModNewEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModNewEntity item)
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
        public int Save(List<ModNewEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(ModNewEntity item)
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
