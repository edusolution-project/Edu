using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class TagsEntity : EntityBase
    {
        [JsonProperty("Name")]
        public String Name { get; set; }
        [JsonProperty("Code")]
        public String Code { get; set; }
        [JsonProperty("ExamQuestionArchiveID")]
        public String ExamQuestionArchiveID { get; set; }
        [JsonProperty("CenterCode")]
        public String CenterCode { get; set; }
        [JsonProperty("CreateUser")]
        public String CreateUser { get; set; }
        [JsonProperty("ParentIDs")]
        public List<String> ParentIDs { get; set; }
    }

    public class TagsService : ServiceBase<TagsEntity>
    {
        public TagsService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<TagsEntity>>
            {
                new CreateIndexModel<TagsEntity>(
                    new IndexKeysDefinitionBuilder<TagsEntity>()
                    .Text(t=> t.Code).Text(t=>t.Name))
            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public TagsEntity GetItemByCode(String code)
        {
            return CreateQuery().Find(x => x.Code.Equals(code)).FirstOrDefault();
        }

        public String GetNamesByCodes(String tags)
        {
            if (tags == null)
            {
                return "";
            }
            else
            {
                var listTags = tags.Split(';');

                if (listTags.Length > 0)
                {
                    var listIDs = new List<String>();
                    listIDs.AddRange(listTags);
                    var newListTags = CreateQuery().Find(x => listIDs.Contains(x.ID)).ToList();
                    String str = "";

                    foreach (var t in newListTags)
                    {
                        str += $"{t.Name}; ";
                    }
                    return str;
                }
                else
                {
                    return "";
                }
            }
        }

        public List<TagsEntity> GetItemByUserAndCenter(String basis, String userID)
        {
            if (String.IsNullOrEmpty(userID))
            {
                return CreateQuery().Find(x => x.CenterCode.Equals(basis)).ToList();
            }
            else
            {
                return CreateQuery().Find(x => x.CenterCode.Equals(basis) && x.CreateUser.Equals(userID)).ToList();
            }
        }

        public List<TagsViewModal> GetList(List<String> ids)
        {
            List<TagsEntity> data = CreateQuery().Find(x => ids.Contains(x.ID)).ToList();
            return (from item in data
                    let tagsVM = new TagsViewModal
                    {
                        id = item.ID,
                        name = item.Name
                    }
                    select tagsVM).ToList();
        }
    }

    public class TagsViewModal
    {
        public String name { get; set; }
        public String id { get; set; }
    }
}
