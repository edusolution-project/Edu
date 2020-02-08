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
        [JsonProperty("Name_vn")]
        public string Name_vn { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
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
                new SkillEntity{ID = "1", Name_vn = "Nghe", Name = "Listening", Image = "/images/skill/listening.png", IsActive = true },
                new SkillEntity{ID = "2", Name_vn = "Nói", Name = "Speaking", Image = "/images/skill/speaking.png", IsActive = true },
                new SkillEntity{ID = "3", Name_vn = "Đọc", Name = "Reading", Image = "/images/skill/reading.png" , IsActive = true},
                new SkillEntity{ID = "4", Name_vn = "Viết", Name = "Writing", Image = "/images/skill/writing.png" , IsActive = true},
                new SkillEntity{ID = "5", Name_vn = "Từ vựng", Name = "Vocabulary", Image = "/images/skill/vocabulary.png", IsActive = true },
                new SkillEntity{ID = "6", Name_vn = "Ngữ pháp", Name = "Grammar", Image = "/images/skill/grammar.png", IsActive = true },
                new SkillEntity{ID = "7", Name_vn = "Tổng hợp", Name = "General", Image = "/images/skill/general.png", IsActive = true },
            };
        }

        public new SkillEntity GetItemByID(string ID)
        {
            return GetList().Find(t => t.ID == ID);
        }
    }
}
