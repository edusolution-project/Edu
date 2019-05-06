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
        protected List<MenuControlAttribute> _currentClient;
        public WebMenu()
        {
            _currentAdmin = GetAdminContrller();
            GetClientContrller(ref _currentClient,ref _listControl);
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
        public  List<MenuControlAttribute> GetClientMenu
        {
            get
            {
                if (_currentClient == null)
                {
                    new WebMenu();
                }
                return _currentClient;
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
                        .Where(o => o.Module.Name == "BasePublisherMVC.dll")
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

        protected void GetClientContrller(ref List<MenuControlAttribute> cusMenus,ref List<CModule> modules)
        {
            var cacheControl = CacheExtends.GetDataFromCache<List<CModule>>(CacheExtends.DefaultIsControl);
            var cacheMenu = CacheExtends.GetDataFromCache<List<MenuControlAttribute>>(CacheExtends.DefaultClientController);
            if (cacheMenu != null && cacheControl != null) {
                cusMenus = cacheMenu;
                modules = cacheControl;
            }
            else
            {
                modules = new List<CModule>();
                cusMenus = new List<MenuControlAttribute>();
                var assembly = Assembly.GetAssembly(typeof(ClientController)).GetTypes()
                    .Where(o => o.BaseType.FullName == typeof(ClientController).FullName)
                    .ToList();
                int count = assembly != null ? assembly.Count : 0;
                for (int i = 0; i < count; i++)
                {
                    var item = assembly[i];
                    var attribute = item.GetCustomAttribute<MenuControlAttribute>();
                    if (attribute == null) continue;
                    if (attribute.IsControl)
                    {
                        var fields =  item.GetFields();
                        List<ProperyCModule> pp = new List<ProperyCModule>();
                        if(fields != null)
                        {
                            foreach(var field in fields)
                            {
                                var attr = field.GetCustomAttribute<PropertyAttribute>();
                                string name = attr.Name;
                                string key = field.Name;
                                string type = attr.Type;
                                pp.Add(new ProperyCModule(key, type, name));
                            }
                        }
                        modules.Add(new CModule() {
                            Code = attribute.CModule,
                            Name = attribute.Name,
                            FullName = item.FullName,
                            Properties = pp
                        });
                    }
                    else
                    {
                        var getmetho = item.GetMethods();
                        var method = getmetho
                            .Where(o => o.Module.Name == "BasePublisherMVC.dll")
                            .Select(o => o.Name).ToList();
                        if (method != null)
                        {
                            foreach (var m in method)
                            {
                                attribute.ActionName.Add(m.ToLower());
                            }
                        }
                        cusMenus.Add(attribute);
                    }
                }
                CacheExtends.SetObjectFromCache(CacheExtends.DefaultIsControl, 360 * 24 * 60, modules);
                CacheExtends.SetObjectFromCache(CacheExtends.DefaultClientController, 360 * 24 * 60, cusMenus);
            }
        }
    }
}
