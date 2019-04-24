using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BaseMVC.Globals
{
    public class MenuControlAttribute : Attribute
    {
        [JsonProperty(PropertyName = "cmodule")]
        public string CModule { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "color")]
        public string Color { get; set; } = "success";
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
}
