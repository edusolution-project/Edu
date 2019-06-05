using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModRatingTeachers")]
    public class ModRatingTeacherEntity : EntityBase
    {
        public string IP { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
    }
    public class ModRatingTeacherService : ServiceBase<ModRatingTeacherEntity>
    {
        #region Private
        public ModRatingTeacherService() : base("ModRatingTeachers")
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
        public Task SetLogin(ModRatingTeacherEntity item)
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

        public ModRatingTeacherEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public ModRatingTeacherEntity GetItemByCode(string token)
        {
            return CreateQuery().SingleOrDefault(o => o.Token == token);
        }
        public ModRatingTeacherEntity GetItemByEmail(string Email)
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
        public List<ModRatingTeacherEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModRatingTeacherEntity item)
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
        public int Save(List<ModRatingTeacherEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
