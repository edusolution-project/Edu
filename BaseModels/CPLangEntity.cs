using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name:"CPLangs")]
    public class CPLangEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Activity { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;
    }
    public class CPLangService : ServiceBase<CPLangEntity>
    {
        #region Private
        public CPLangService() : base("CPLangs")
        {

        }
        #endregion
        public CPLangEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public CPLangEntity GetItemByCode(string code)
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
        public List<CPLangEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(CPLangEntity item)
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
        public async Task<int> SaveAsync(CPLangEntity item)
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
        public int Save(List<CPLangEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
