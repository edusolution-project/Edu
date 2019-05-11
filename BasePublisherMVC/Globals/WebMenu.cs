using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BasePublisherMVC.Globals
{
    public class CModule
    {
        public string Name { get; set; }
        public string Code { get; set; } // CAdv
        public string FullName { get; set; } //Admin.cotroller.CAdv
        public List<ProperyCModule> Properties { get; set; }
    }
    public class ProperyCModule
    {
        public ProperyCModule()
        {
        }

        public ProperyCModule(string key, string type, string name)
        {
            Key = key;
            Type = type;
            Name = name;
        }
        public object Value { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
    }
    public class WebMenu
    {
        protected List<CModule> _listControl;
        protected List<MenuControlAttribute> _currentAdmin;
        public WebMenu()
        {
            _currentAdmin = GetAdminContrller();
        }
        public  List<MenuControlAttribute> GetAdminMenu {
            get
            {
                if(_currentAdmin == null)
                {
                    new WebMenu();
                }
                return _currentAdmin;
            }
        }
        public List<CModule> GetControl
        {
            get
            {
                if (_listControl == null)
                {
                    new WebMenu();
                }
                return _listControl;
            }
        }
        protected List<MenuControlAttribute> GetAdminContrller()
        {
            var cacheMenu = CacheExtends.GetDataFromCache<List<MenuControlAttribute>>(CacheExtends.DefaultAdminController);
            if (cacheMenu != null) return cacheMenu;
            else
            {
                var cusMenus = new List<MenuControlAttribute>();
                var assembly = Assembly.GetAssembly(typeof(AdminController)).GetTypes()
                    .Where(o => o.BaseType.FullName == typeof(AdminController).FullName)
                    .ToList();
                int count = assembly != null ? assembly.Count : 0;
                for (int i = 0; i < count; i++)
                {
                    var item = assembly[i];
                    var attribute = item.GetCustomAttribute<MenuControlAttribute>();
                    if (attribute == null) continue;
                    var getmetho = item.GetMethods();
                    var method = getmetho
                        .Where(o => o.Module.Name == typeof(AdminController).Module.Name)
                        .Select(o => o.Name).ToList();
                    if (method != null)
                    {
                        foreach (var m in method)
                        {
                            attribute.ActionName.Add(m.ToLower());
                        }
                    }
                    attribute.Color = string.IsNullOrEmpty(attribute.Color) ? new GetColor().RandomColor() : attribute.Color;
                    cusMenus.Add(attribute);
                }
                CacheExtends.SetObjectFromCache(CacheExtends.DefaultAdminController, 360 * 24 * 60, cusMenus);

                return cusMenus;
            }
        }
    }
}
