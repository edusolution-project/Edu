using System;
using System.Collections.Generic;
using System.Text;

namespace BaseAccess.Attribule
{
    public class AccessCtrlAttribute : Attribute
    {
        public AccessCtrlAttribute(string name, string module, string type, bool isShow) : this(name, module, type)
        {
            IsShow = isShow;
        }
        public AccessCtrlAttribute(string name, string module, string type)
        {
            Name = name;
            Module = module;
            Type = type;
            IsShow = true;
        }
        public AccessCtrlAttribute(string name, bool isShow)
        {
            Name = name;
            IsShow = isShow;
        }
        public AccessCtrlAttribute(string name,  string type)
        {
            Name = name;
            Type = type;
            IsShow = true;
        }

        public string Name { get; set; }
        public string Module { get; set; }
        public string Type { get; set; } 
        public bool IsShow { get; set; }
        public HashSet<string> Acts { get; set; } = new HashSet<string>();
    }
}
