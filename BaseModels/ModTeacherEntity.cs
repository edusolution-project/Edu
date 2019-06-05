using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModTeachers")]
    public class ModTeacherEntity : EntityBase
    {
        public string IP { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
    }
    public class ModTeacherService : ServiceBase<ModTeacherEntity>
    {
        #region Private
        public ModTeacherService() : base("ModTeachers")
        {

        }
        #endregion

        public Task RemoveLog(string token)
        {
            var item = GetItemByCode(token);
            if (item == null) return Task.CompletedTask;
            CreateQuery().Remove(item);
            CreateQuery().Complete();
            return Task.CompletedTask;
        }
        public Task SetLogin(ModTeacherEntity item)
        {
            if (item == null) return Task.CompletedTask;
            var current = GetItemByEmail(item.Email);
            if (current == null)
            {
                CreateQuery().Add(item);
                CreateQuery().Complete();
                return Task.CompletedTask;
            }
            else
            {
                CreateQuery().Update(current,item);
                CreateQuery().Complete();
                return Task.CompletedTask;
            }
        }
        public string GetEmailFromDb(string token)
        {
            var data = CreateQuery().SingleOrDefault(o => o.Activity == true && o.Token == token);
            return data == null ? string.Empty : data.Email;
        }

        public ModTeacherEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public ModTeacherEntity GetItemByCode(string token)
        {
            return CreateQuery().SingleOrDefault(o => o.Token == token);
        }
        public ModTeacherEntity GetItemByEmail(string Email)
        {
            return CreateQuery().SingleOrDefault(o => o.Email == Email);
        }
        public bool IsExistByEmail(string Email)
        {
            return GetItemByEmail(Email) != null;
        }
        public bool IsExistByID(int ID)
        {
            return GetItemByID(ID) != null;
        }
        public bool IsExistByCode(string code)
        {
            return GetItemByCode(code) != null;
        }
        public List<ModTeacherEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModTeacherEntity item)
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
        public int Save(List<ModTeacherEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
