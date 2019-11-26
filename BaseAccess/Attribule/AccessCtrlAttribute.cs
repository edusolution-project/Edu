using System;
using System.Collections.Generic;
using System.Text;

namespace BaseAccess.Attribule
{
    public class AccessCtrlAttribute : Attribute
    {
        public AccessCtrlAttribute(string name, string module, string type)
        {
            Name = name;
            Module = module;
            Type = type;
        }

        public string Name { get; set; }
        public string Module { get; set; }
        public string Type { get; set; } 
        public HashSet<string> Acts { get; set; } = new HashSet<string>();
    }
}
