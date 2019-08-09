using BaseCustomerEntity.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace BaseCustomerMVC.Models
{
    public class ScoreSummaryViewModel
    {
        [JsonProperty("StudentID")]
        public string StudentID { get; set; }
        [JsonProperty("StudentName")]
        public string StudentName { get; set; }
        [JsonProperty("Scores")]
        public List<ScoreEntity> Scores { get; set; }
        [JsonProperty("AvgScore")]
        public double AvgScore { get; set; }

        [JsonProperty("PartialScore")]
        public double PartialScore { get; set; }

        [JsonProperty("PartialSum")]
        public double PartialSum { get; set; }

        [JsonProperty("AvgPartial")]
        public double AvgPartial { get; set; }

        [JsonProperty("EndingScore")]
        public double EndingScore { get; set; }

        [JsonProperty("EndingSum")]
        public double EndingSum { get; set; }

        [JsonProperty("AvgEnd")]
        public double AvgEnd { get; set; }

    }
}
