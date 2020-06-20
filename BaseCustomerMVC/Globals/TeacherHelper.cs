using BaseCustomerEntity.Database;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class TeacherHelper
    {
        private readonly TeacherService _teacherService;
        private readonly AccountService _accountService;

        public TeacherHelper(
            TeacherService teacherService,
            AccountService accountService
        )
        {
            _accountService = accountService;
            _teacherService = teacherService;
        }

        public void ChangeStatus(string id, bool status)
        {
            try
            {
                var filter = Builders<TeacherEntity>.Filter.Where(o => o.ID == id);
                var update = Builders<TeacherEntity>.Update.Set("IsActive", status);
                var publish = _teacherService.Collection.UpdateManyAsync(filter, update);

                var filterAcc = Builders<AccountEntity>.Filter.Where(o => o.UserID == id && o.Type == ACCOUNT_TYPE.TEACHER);
                var updateAcc = Builders<AccountEntity>.Update.Set("IsActive", status);
                _accountService.CreateQuery().UpdateManyAsync(filterAcc, updateAcc);
            }
            catch (Exception)
            {
                //log exception
            }
        }

        public void ChangeStatus(string[] ids, bool status)
        {
            var filter = Builders<TeacherEntity>.Filter.Where(o => ids.Contains(o.ID));
            var update = Builders<TeacherEntity>.Update.Set("IsActive", status);
            var publish = _teacherService.Collection.UpdateManyAsync(filter, update);

            var filterAcc = Builders<AccountEntity>.Filter.Where(o => ids.Contains(o.UserID) && o.Type == ACCOUNT_TYPE.TEACHER);
            var updateAcc = Builders<AccountEntity>.Update.Set("IsActive", status);
            _accountService.CreateQuery().UpdateManyAsync(filterAcc, updateAcc);
        }

    }
}
