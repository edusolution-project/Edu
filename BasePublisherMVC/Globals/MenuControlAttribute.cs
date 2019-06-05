using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BasePublisherMVC.Globals
{
    public class MenuControlAttribute : Attribute
    {
        [JsonProperty(PropertyName = "cmodule")]
        public string CModule { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; }
        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }
        [JsonProperty(PropertyName = "icon")]
        public string Icon { get; set; } = "dashboard";
        [JsonProperty(PropertyName = "type")]
        public MenuType Type { get; set; } = MenuType.Sys;
        [JsonProperty(PropertyName = "show")]
        public bool IShow { get; set; } = true;
        [JsonProperty(PropertyName = "actions")]
        public HashSet<string> ActionName { get; set; } = new HashSet<string>();
        [JsonProperty(PropertyName = "iscontrol")]
        public bool IsControl { get; set; } = false;
    }
    public enum MenuType
    {
        Mod, // ClientCtroller
        Sys  // AdminController
    }
    public class GetColor{
        public string RandomColor()
        {
            var _color = new List<string>();
            var cacle = CacheExtends.GetDataFromCache<List<string>>("colorGet");
            if(_color.Count == 0)
            {
                _color = cacle != null && cacle.Count > 0? cacle : new List<string>() { "cyan", "red", "emerald", "indigo", "violet", "teal" };
            }
            Random random = new Random();
            int id = random.Next(0, _color.Count);
            var colorReturn = _color[id].ToString();
            _color.Remove(colorReturn);
            CacheExtends.SetObjectFromCache("colorGet", 240,_color);
            return colorReturn;
        }
    }
}

