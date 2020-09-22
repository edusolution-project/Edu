using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseCustomerEntity.Database
{
    public class TransactionEntity:EntityBase
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }

        [JsonProperty("NewsID")] //mua sản phẩm nào(type="san-pham")
        public string NewsID { get; set; }

        [JsonProperty("CenterID")] //mua ở cơ sở nào
        public string CenterID { get; set; }

        [JsonProperty("DayBuy")] //ngày mua
        public DateTime DayBuy { get; set; }

        [JsonProperty("Price")]
        public double Price { get; set; }
         
        [JsonProperty("TradingID")] //mã giao dịch, lấy từ ngân hàng
        public string TradingID { get; set; }
        [JsonProperty("DayPayment")] //thoi gian thuc hien thanh toan
        public DateTime DayPayment { get; set; }
        [JsonProperty("StatusPayment")] //thoi gian thuc hien thanh toan
        public bool StatusPayment { get; set; }
        [JsonProperty("MsgPayment")] //thông báo của trạng thái thanh toán
        public string MsgPayment { get; set; }
    }

    public class TransactionService: ServiceBase<TransactionEntity>
    {
        private readonly IndexService _indexService;
        public TransactionService(IConfiguration config, IndexService indexService) : base(config)
        {
            _indexService = indexService;
            var indexs = new List<CreateIndexModel<TransactionEntity>> { };
            Collection.Indexes.CreateManyAsync(indexs);
        }
    }
}
