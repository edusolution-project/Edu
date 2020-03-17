using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseEasyRealTime.Entities
{
    public class NotificationEntity : EntityBase
    {
        public string GroupName { get; set; }
        public string MessageCode { get; set; }
        public HashSet<string> UserViews { get; set; } = new HashSet<string>();
        public bool? IsPrivated { get; set; }
        public DateTime Created { get; set; }
    }

    public class NotificationService : ServiceBase<NotificationEntity>
    {
        public NotificationService(IConfiguration config) : base(config)
        {

        }

        public IEnumerable<NotificationEntity> GetListNoViews(string userID)
        {
            return CreateQuery().Find(o=>o.UserViews.Contains(userID))?.ToList();
        }
        public IEnumerable<string> GetViews(string groupName, string messageCode)
        {
            var item = CreateQuery().Find(o => o.GroupName == groupName && o.MessageCode == messageCode)?.FirstOrDefault();
            if (item == null) return null;
            return item.UserViews;
        }

        public NotificationEntity UpdateView(string groupName, string messageCode, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            var item = CreateQuery().Find(o => o.GroupName == groupName && o.MessageCode == messageCode)?.FirstOrDefault();
            if (item == null) return null;
            else
            {
                if (item.UserViews.Contains(userId))
                {
                    item.UserViews.Remove(userId);
                }
                if (item.UserViews.Count > 0)
                {
                    CreateQuery().ReplaceOne(Builders<NotificationEntity>.Filter.Eq(o => o.ID, item.ID), item);
                }
                else
                {
                    Remove(item.ID);
                }
            }
            return item;
        }

        public bool Create(NotificationEntity item)
        {
            if(GetViews(item.GroupName,item.MessageCode) == null)
            {
                item.Created = DateTime.Now;
                CreateOrUpdate(item);
                return true;
            }
            return false;
        }
    }
}
