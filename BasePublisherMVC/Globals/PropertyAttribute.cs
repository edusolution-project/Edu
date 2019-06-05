using System;
using System.Collections.Generic;
using System.Text;

namespace BasePublisherMVC.Globals
{
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(string name)
        {
            Name = name;
        }

        public PropertyAttribute(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; set; }
        public string Type { get; set; }
       
    }
}
