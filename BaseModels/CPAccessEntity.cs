using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "CPAccesses")]
    public class CPAccessEntity : EntityBase
    {
        public string CModule { get; set; }
        public string CMethod { get; set; }
        public int RoleID { get; set; }
        public bool Activity { get; set; }

        public virtual CPRoleEntity GetRole()
        {
            var role = new CPRoleService();
            return role.GetItemByID(RoleID);
        }
    }
    public class CPAccessService : ServiceBase<CPAccessEntity>
    {
        #region Private
        public CPAccessService() : base("CPAccesses")
        {

        }
        #endregion
        public bool GetPermission(int roleID,string ctrlName,string actName)
        {
            var data = GetItem(roleID, ctrlName, actName);
            if (data == null) return false;
            else return data.Activity;
        }
        public CPAccessEntity GetItem(int roleID, string ctrlName, string actName)
        {
            var data = CreateQuery().SingleOrDefault(o => o.RoleID == roleID && o.CModule == ctrlName && o.CMethod == actName);
            return data??null;
        }
        public Dictionary<string,bool> GetPermissions(int roleID)
        {
            var data = CreateQuery().Find(o => o.RoleID == roleID).ToList();
            if (data == null) return null;
            else
            {
                var res = new Dictionary<string, bool>();
                int count = data.Count();
                for(int i = 0; i < count; i++)
                {
                    var item = data[i];
                    res.Add(item.CModule + "-" + item.CMethod, item.Activity);
                }
                return res;
            }
        }
        public CPAccessEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public List<CPAccessEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public List<CPAccessEntity> GetItemByRoleID(int roleID)
        {
            return CreateQuery().Find(o => o.RoleID == roleID).ToList();
        }
        public int Save(CPAccessEntity item)
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
                    CreateQuery().Update(current,item);
                    return CreateQuery().Complete();
                }
                else
                {
                    return -1;
                }
            }
        }
        public int Save(List<CPAccessEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(CPAccessEntity item)
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
