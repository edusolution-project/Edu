using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseEasyRealTime.Entities
{
    public class NewFeedEntity : MessageEntity
    {
        public string Title { get; set; }
        public string Name { get; set; }
    }
    public class NewFeedService : ServiceBase<NewFeedEntity>
    {
        public NewFeedService(IConfiguration config) : base(config)
        {
        }
        public new NewFeedEntity GetItemByID(string id)
        {
            var item = Collection.Find(o => o.RemoveByAdmin == false && o.ID == id)?.SingleOrDefault();
            return item;
        }
    }
}
