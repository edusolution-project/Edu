using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BaseCustomerMVC.Models
{
    public class ExamReviewViewModel : ExamEntity
    {
        [JsonProperty("Details")]
        public List<ExamDetailEntity> Details { get; set; }
    }
}
