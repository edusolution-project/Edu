﻿using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    [Serializable]
    public class SkillEntity : EntityBase
    {
        [JsonProperty("Name_vn")]
        public string Name_vn { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("Color")]
        public string Color { get; set; }
        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }
    }
    public class SkillService : ServiceBase<SkillEntity>
    {
        public SkillService(IConfiguration config) : base(config)
        {
        }

        public List<SkillEntity> GetList()
        {
            return new List<SkillEntity>
            {
                new SkillEntity{ID = "1", Name_vn = "Nghe", Name = "Listening", Image = "/images/skill/listening.png", IsActive = true, Color = "#00BBD4" },
                new SkillEntity{ID = "2", Name_vn = "Nói", Name = "Speaking", Image = "/images/skill/speaking.png", IsActive = true, Color = "#B70058" },
                new SkillEntity{ID = "3", Name_vn = "Đọc", Name = "Reading", Image = "/images/skill/reading.png" , IsActive = true, Color = "#00D9A5"},
                new SkillEntity{ID = "4", Name_vn = "Viết", Name = "Writing", Image = "/images/skill/writing.png" , IsActive = true, Color = "#F7971E"},
                new SkillEntity{ID = "5", Name_vn = "Từ vựng", Name = "Vocabulary", Image = "/images/skill/vocabulary.png", IsActive = true, Color = "#007B83" },
                new SkillEntity{ID = "6", Name_vn = "Ngữ pháp", Name = "Grammar", Image = "/images/skill/grammar.png", IsActive = true, Color = "#D03239" },
                new SkillEntity{ID = "7", Name_vn = "Tổng hợp", Name = "General", Image = "/images/skill/general.png", IsActive = true, Color = "#47d9a5" },
                new SkillEntity{ID = "8", Name_vn = "CDIO", Name = "CDIO", Image = "/images/skill/general.png", IsActive = true, Color = "#47d9a5" },
                new SkillEntity{ID = "9", Name_vn = "Elearning", Name = "Elearning", Image = "/images/skill/elearning.png", IsActive = true, Color = "#34b2dc" },
            };
        }

        public new SkillEntity GetItemByID(string ID)
        {
            return GetList().Find(t => t.ID == ID);
        }
    }
}
