using Core_v2.Globals;
using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class FileManagerEntity : EntityBase
    {
        [JsonProperty("FileID")]
        public string FileID { get; set; }
        [JsonProperty("FolderID")]
        public string FolderID { get; set; }
        [JsonProperty("Extends")]
        public string Extends { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("Center")]
        public string Center { get; set; }
    }
    public class FileManagerService : ServiceBase<FileManagerEntity>
    {
        public FileManagerService(IConfiguration configuration) : base(configuration)
        {

        }
        public FileManagerEntity GetItem(string center, string user, string fileId)
        {
            return CreateQuery().Find(o => o.Center == center && o.UserID == user && o.FileID == fileId)?.SingleOrDefault();
        }
        public bool RemoveFile(string center, string user, string fileId)
        {
            var item = GetItem(center, user, fileId);
            if (item != null)
            {
                var delete = CreateQuery().DeleteOne(o => o.ID == item.ID);
                return delete.DeletedCount > 0;
            }

            return false;
        }
    }

    public class FolderCenterEntity : EntityBase
    {
        [JsonProperty("Center")]
        public string Center { get; set; }
        [JsonProperty("FolderID")]
        public string FolderID { get; set; }
        [JsonProperty("IsRoot")]
        public bool IsRoot { get; set; } = false;
    }


    public class FolderCenterService : ServiceBase<FolderCenterEntity>
    {
        public FolderCenterService(IConfiguration configuration) : base(configuration)
        {

        }

        public string GetFolderID(string center)
        {
            var item = CreateQuery().Find(o => o.Center == center)?.FirstOrDefault();
            return item == null ? string.Empty :item.FolderID;
        }

        public string GetRoot()
        {
            var item = CreateQuery().Find(o => o.IsRoot == true)?.FirstOrDefault();
            return item == null ? string.Empty : item.FolderID;
        }

        public void CreateRoot(string folderId)
        {
            var item = new FolderCenterEntity()
            {
                Center = "EDUSO_ROOT",
                FolderID = folderId,
                IsRoot = true
            };

            CreateQuery().InsertOne(item);
        }

    }

    public class FolderManagerEntity : EntityBase
    {
        [JsonProperty("Center")]
        public string Center { get; set; }
        [JsonProperty("UserID")]
        public string UserID { get; set; }
        [JsonProperty("FolderID")]
        public string FolderID { get; set; }
    }
    public class FolderManagerService : ServiceBase<FolderManagerEntity>
    {
        public FolderManagerService(IConfiguration configuration) : base(configuration)
        {

        }

        public string GetFolderID(string center,string user)
        {
            var item = CreateQuery().Find(o => o.Center == center && o.UserID == user)?.FirstOrDefault();
            return item == null ? string.Empty : item.FolderID;
        }
    }
}
