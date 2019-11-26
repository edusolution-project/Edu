using BaseCustomerEntity.Database;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class ProgressHelper
    {
        private readonly StudentService _studentService;
        private readonly AccountService _accountService;

        public ProgressHelper(
            StudentService studentService,
            AccountService accountService
        )
        {
            _accountService = accountService;
            _studentService = studentService;
        }

        public void ChangeStatus(string id, bool status)
        {
            try
            {
                var filter = Builders<StudentEntity>.Filter.Where(o => o.ID == id);
                var update = Builders<StudentEntity>.Update.Set("IsActive", status);
                var publish = _studentService.Collection.UpdateManyAsync(filter, update);

                var filterAcc = Builders<AccountEntity>.Filter.Where(o => o.UserID == id && o.Type == ACCOUNT_TYPE.STUDENT);
                var updateAcc = Builders<AccountEntity>.Update.Set("IsActive", status);
                _accountService.CreateQuery().UpdateManyAsync(filterAcc, updateAcc);
            }
            catch (Exception e)
            {
                //log exception
            }
        }

        public void ChangeStatus(string[] ids, bool status)
        {
            var filter = Builders<StudentEntity>.Filter.Where(o => ids.Contains(o.ID));
            var update = Builders<StudentEntity>.Update.Set("IsActive", status);
            var publish = _studentService.Collection.UpdateMany(filter, update);

            var filterAcc = Builders<AccountEntity>.Filter.Where(o => ids.Contains(o.UserID) && o.Type == ACCOUNT_TYPE.STUDENT);
            var updateAcc = Builders<AccountEntity>.Update.Set("IsActive", status);
            _accountService.CreateQuery().UpdateMany(filterAcc, updateAcc);
        }

    }
}
