﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class NewsCategoryEntity : EntityBase
    {
        [JsonProperty("UID")]
        public string UID { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("ParentID")]
        public string ParentID { get; set; }
    }

    public class NewsCategoryService : ServiceBase<NewsCategoryEntity>
    {
        private readonly IndexService _indexService;

        public NewsCategoryService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<NewsCategoryEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        //public new NewsCategoryEntity Save(NewsCategoryEntity entity)
        //{
        //    if (entity.ID == null || entity.ID == "")
        //    {
        //        entity.UID = _indexService.GetNewIndex("NewsCategory").ToString();
        //        Collection.InsertOne(entity);
        //    }
        //    else
        //    {
        //        Collection.ReplaceOne(t => t.ID == entity.ID, entity);
        //    }
        //    return entity;
        //}

        public NewsCategoryEntity GetItemByCode(string code) => Collection.Find<NewsCategoryEntity>(x => x.Code.Equals(code)).FirstOrDefault();

        //public NewsCategoryEntity getNameCategoryByID(string ID) => Collection.Find<NewsCategoryEntity>(x => x.ID.Equals(ID)).FirstOrDefault();
    }
}
