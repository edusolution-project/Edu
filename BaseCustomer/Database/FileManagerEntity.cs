using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
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
    }
    public class FileManagerService : ServiceBase<FileManagerEntity>
    {
        public FileManagerService(IConfiguration configuration) : base(configuration)
        {

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
    }
}
