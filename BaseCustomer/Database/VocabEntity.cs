using Core_v2.Repositories;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseCustomerEntity.Database
{
    public class VocabularyEntity : EntityBase
    {
        [JsonProperty("Language")]
        public string Language { get; set; }
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("Code")]
        public string Code { get; set; }
        [JsonProperty("Description")]
        public string Description { get; set; }
        [JsonProperty("Image")]
        public string Image { get; set; }
        [JsonProperty("Pronunciation")]
        public string Pronunciation { get; set; }
        [JsonProperty("PronunAudioPath")]
        public string PronunAudioPath { get; set; }
        [JsonProperty("WordType")]
        public string WordType { get; set; }
        [JsonProperty("Created")]
        public DateTime Created { get; set; }

    }

    public class VocabularyService : ServiceBase<VocabularyEntity>
    {
        public VocabularyService(IConfiguration config) : base(config)
        {
            var indexs = new List<CreateIndexModel<VocabularyEntity>>
            {

            };

            Collection.Indexes.CreateManyAsync(indexs);
        }

        public List<VocabularyEntity> GetItemByCode(string code)
        {
            return Collection.Find(t => t.Code == code).ToList();
        }
    }

    public class WordType
    {
        public const string adjective = "adj", adverb = "adv",
            conjunction = "con", noun = "n", phrasal_verb = "ph.v", preposition = "pre", verb = "v";

        public static string GetShort(string fullStr)
        {
            switch(fullStr)
            {
                case "adjective":
                    return adjective;
                case "adverb":
                    return adverb;
                case "conjunction":
                    return conjunction;
                case "noun":
                    return noun;
                case "preposition":
                    return preposition;
                case "verb":
                    return verb;
                default:
                    return phrasal_verb;
            }
        }
    }
}
