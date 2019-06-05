using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "CPUsers")]
    public class CPUserEntity : EntityBase
    {
        public string Name { get; set; }
        public int RoleID { get; set; } = 2;
        public string Email { get; set; }
        public string Pass { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime BirthDay { get; set; }
        public string Skype { get; set; }
        public string Phone { get; set; }
        public bool Activity { get; set; }
        [Column(TypeName = "DateTime2")]
        public DateTime Created { get; set; } = DateTime.Now;

        public CPRoleEntity CurrentRole
        {
            get {
                var role = new CPRoleService();
                return role.GetItemByID(RoleID);
            }
        }
        public string CurrentRoleName => CurrentRole.Name;
    }
    public class CPUserService : ServiceBase<CPUserEntity>
    {
        #region Private
        public CPUserService() : base("CPUsers")
        {

        }
        #endregion
        public CPUserEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public CPUserEntity GetItemByEmail(string email)
        {
            return CreateQuery().SingleOrDefault(o => o.Email == email);
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public bool IsExistByEmail(string Email)
        {
            return GetItemByEmail(Email) != null;
        }
        public List<CPUserEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(CPUserEntity item)
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
        public int Save(List<CPUserEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
        public async Task<int> SaveAsync(CPUserEntity item)
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
