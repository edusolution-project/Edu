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
        private readonly RoleService _roleService;

        public TeacherHelper(
            TeacherService teacherService,
            AccountService accountService,
            RoleService roleService
        )
        {
            _accountService = accountService;
            _teacherService = teacherService;
            _roleService = roleService;
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
            catch (Exception e)
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

        public bool HasRole(string userid, string center, string role)
        {
            var teacher = _teacherService.GetItemByID(userid);
            if (teacher == null) return false;
            var centerMember = teacher.Centers.Find(t => t.CenterID == center);
            if (centerMember == null) return false;
            if (_roleService.GetItemByID(centerMember.RoleID).Code != role) return false;
            return true;
        }

    }
}
