using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "CPRoles")]
    public class CPRoleEntity : EntityBase
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Lock { get; set; } = false;
    }
    public class CPRoleService : ServiceBase<CPRoleEntity>
    {
        #region Private
        public CPRoleService() : base("CPRoles")
        {

        }
        #endregion
        public CPRoleEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public CPRoleEntity GetItemByCode(string code)
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
        public List<CPRoleEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(CPRoleEntity item)
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
        public int Save(List<CPRoleEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(CPRoleEntity item)
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
