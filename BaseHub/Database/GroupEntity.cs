using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaseHub.Database
{
    public class GroupEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("DisplayName")]
        public string DisplayName { get; set; }
        [JsonProperty("Members")]
        public List<string> Members { get; set; } = new List<string>();
        [JsonProperty("Created")]
        public DateTime Created { get; set; }

    }
    public class GroupService : ServiceBase<GroupEntity>
    {
        public string SplitSymbol = "__&&__";
        public GroupService(IConfiguration config) : base(config)
        {

        }
        [Obsolete]
        public void Create(GroupEntity item)
        {
            var _oldItem = CreateQuery().Find(o => o.Name == item.Name).Count();
            if(_oldItem > 0)
            {
                return;
            }
            else
            {
                item.Created = DateTime.Now;
                CreateOrUpdate(item);
            }
        }
        [Obsolete]
        public GroupEntity GetItemByName(string groupName)
        {
            var _item = CreateQuery().Find(o => o.Name == groupName);
            if (_item.Count() <= 0 || _item == null) return null;
            return _item?.First();
        }
        [Obsolete]
        public Task AddMember(string groupName,string userID)
        {
            char _sp = new char() {  };
            string _groupName = "";
            if (groupName.Contains(this.SplitSymbol))
            {
                var spGr = Regex.Split(groupName, SplitSymbol);
                _groupName = spGr[1] + SplitSymbol + spGr[0];
            }
            var _item = CreateQuery().Find(o => o.Name == groupName || (!string.IsNullOrEmpty(_groupName) && o.Name == _groupName));
            if(_item == null || _item.Count() <= 0)
            {
                var item = new GroupEntity()
                {
                    Created = DateTime.Now,
                    DisplayName = "",
                    Members = new List<string>() { userID },
                    Name= groupName
                };
                CreateOrUpdate(item);
            }
            else
            {
                var first = _item.First();
                if (!first.Members.Contains(userID))
                {
                    first.Members.Add(userID);
                    CreateOrUpdate(first);
                }
            }
            return Task.CompletedTask;
        }
        public void RemoveMember(string groupName, string userID)
        {
            var _item = CreateQuery().Find(o => o.Name == groupName).First();
            if (_item == null)
            {
                return;
            }
            else
            {
                if (!_item.Members.Contains(userID))
                {
                    _item.Members.Remove(userID);
                    CreateOrUpdate(_item);
                }
            }
            return;
        }
    }
}
