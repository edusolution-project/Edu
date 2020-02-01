using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseCustomerEntity.Database
{
    public class SkillEntity : EntityBase
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Name_en")]
        public string Name_en { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
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
                new SkillEntity{ID = "1", Name = "Nghe", Name_en = "Listening", Image = "/images/skill/listening.png" },
                new SkillEntity{ID = "2", Name = "Nói", Name_en = "Speaking", Image = "/images/skill/speaking.png" },
                new SkillEntity{ID = "3", Name = "Đọc", Name_en = "Reading", Image = "/images/skill/reading.png" },
                new SkillEntity{ID = "4", Name = "Viết", Name_en = "Writing", Image = "/images/skill/writing.png" },
                new SkillEntity{ID = "5", Name = "Từ vựng", Name_en = "Vocabulary", Image = "/images/skill/vocabulary.png" },
                new SkillEntity{ID = "6", Name = "Ngữ pháp", Name_en = "Grammar", Image = "/images/skill/grammar.png" },
                new SkillEntity{ID = "7", Name = "Tổng hợp", Name_en = "General", Image = "/images/skill/general.png" },
            };
        }

        public new SkillEntity GetItemByID(string ID)
        {
            return GetList().Find(t => t.ID == ID);
        }
    }
}
