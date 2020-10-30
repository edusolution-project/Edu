using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class ReportEntity:EntityBase
    {
        [JsonProperty("CenterID")]
        public String CenterID { get; set; }
        [JsonProperty("ClassID")]
        public String ClassID { get; set; }
        [JsonProperty("Students")]
        public Int32 Students { get; set; }//Sĩ số
        [JsonProperty("InactiveStudents")]
        public Int32 InactiveStudents { get; set; }//Học sinh chưa học
        [JsonProperty("MinPoint8")]
        public Double MinPoint8 { get; set; }//Điểm từ 8 -> 10
        [JsonProperty("MinPoint5")]
        public Double MinPoint5 { get; set; }//Điểm từ 5 -> 7
        [JsonProperty("MinPoint2")]
        public Double MinPoint2 { get; set; }//Điểm từ 2 -> 5
        [JsonProperty("MinPoint0")]
        public Double MinPoint0 { get; set; }//Điểm từ 0 -> 2
        [JsonProperty("TimeExport")]
        public DateTime TimeExport { get; set; }//THời gian xuất
        [JsonProperty("StartDate")]
        public DateTime StartDate { get; set; }//THời gian bắt đầu
        [JsonProperty("EndDate")]
        public DateTime EndDate { get; set; }//Thời gian kết thúc

        [JsonProperty("ClassName")]
        public String ClassName { get; set; }
    }

    public class ReportService : ServiceBase<ReportEntity>
    {
        private readonly IndexService _indexService;
        public ReportService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<ReportEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }

        public IEnumerable<ReportEntity> GetReport(DateTime TimeExport, String centerID, String classID = null)
        {
            if (String.IsNullOrEmpty(classID))
            {
                return CreateQuery().Find(x => x.CenterID == centerID && x.TimeExport == TimeExport).ToEnumerable();
            }
            else
            {
                return CreateQuery().Find(x => x.CenterID == centerID && x.ClassID == classID && x.TimeExport == TimeExport).ToEnumerable();
            }
            
        }
    }
}
