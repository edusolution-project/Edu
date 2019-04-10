using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using CoreEDB.CoreModels;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace BaseModels
{
    [Table(name: "ModExamples")]
    public class ModExampleEntity : EntityBase
    {
        public string Name { get; set; }
        public int MenuID { get; set; }
        public bool IsAdmin { get; set; } // giáo viên tao . hoặc admin tao
        public int CreateUser { get; set; }
        public bool Activity { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class ModExampleService : ServiceBase<ModExampleEntity>
    {
        #region Private
        public ModExampleService() : base("ModExamples")
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
        public Task SetLogin(ModExampleEntity item)
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

        public ModExampleEntity GetItemByID(int ID)
        {
            return CreateQuery().Get(ID);
        }
        public ModExampleEntity GetItemByCode(string token)
        {
            return CreateQuery().SingleOrDefault(o => o.Token == token);
        }
        public ModExampleEntity GetItemByEmail(string Email)
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
        public List<ModExampleEntity> GetAllItem()
        {
            return CreateQuery().GetAll().ToList();
        }
        public int Save(ModExampleEntity item)
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
        public int Save(List<ModExampleEntity> item)
        {
            CreateQuery().AddRange(item);
            return CreateQuery().Complete();
        }
    }
}
