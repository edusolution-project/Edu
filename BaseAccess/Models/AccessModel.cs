using System.Collections.Generic;

namespace BaseAccess.Models
{
    public class AccessModel
    {
        public string Group { get; set; }
        public string Ctrl { get; set; }
        public HashSet<string> Acts { get; set; }
    }
}
