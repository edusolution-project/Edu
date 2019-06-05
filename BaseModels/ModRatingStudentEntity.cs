using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModRatingStudents")]
    public class ModRatingStudentEntity : EntityBase
    {
        public string IP { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
    }
    public class ModRatingStudentService : ServiceBase<ModRatingStudentEntity>
    {
        #region Private
        public ModRatingStudentService() : base("ModRatingStudents")
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
        public Task SetLogin(ModRatingStudentEntity item)
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

        public ModRatingStudentEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public ModRatingStudentEntity GetItemByCode(string token)
        {
            return CreateQuery().SingleOrDefault(o => o.Token == token);
        }
        public ModRatingStudentEntity GetItemByEmail(string Email)
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
        public List<ModRatingStudentEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModRatingStudentEntity item)
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
        public int Save(List<ModRatingStudentEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
