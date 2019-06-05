using System;

namespace BaseMVC.Models
{
    public class LayoutModel
    {
        public string PartialID { get; set; } = Guid.NewGuid().ToString();
        public string LayoutName { get; set; }
        public string CModule { get; set; }
        public object Properties { get; set; }
        public bool IsDynamic { get; set; } = false;
        public bool IsBody { get; set; } = false;
        public string ParrentLayout { get; set; }
        public int Order { get; set; } = 999;
        public string PartialView { get; set; }
        public string TypeView { get; set; }
    }
}
