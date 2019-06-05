using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "CPResources")]
    public class CPResourceEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public int LangID { get; set; }
    }
    public class CPResourceService : ServiceBase<CPResourceEntity>
    {
        #region Private
        public CPResourceService() : base("CPResources")
        {

        }
        #endregion
        public CPResourceEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public CPResourceEntity GetItemByCode(string code)
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
        public List<CPResourceEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public List<CPResourceEntity> GetByLangID(int LangID)
        {
            return CreateQuery().Find(o=>o.LangID == LangID).ToList();
        }
        public int Save(CPResourceEntity item)
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
        public int Save(List<CPResourceEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(CPResourceEntity item)
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
