using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
   public class NewsCategoryEntity: EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
    }

    public class NewsCategoryService : ServiceBase<NewsCategoryEntity>
    {
        public NewsCategoryService(IConfiguration config):base(config)
        {
            var indexs = new List<CreateIndexModel<NewsCategoryEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public NewsCategoryEntity getNameCategoryByID(string ID) => Collection.Find<NewsCategoryEntity>(x => x.ID.Equals(ID)).FirstOrDefault();
    }
}
