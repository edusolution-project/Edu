using Core_v2.Globals;
using Core_v2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseCustomerMVC.Globals
{
    public class IndefindCtrlAttribulte : Attribute
    {
        public IndefindCtrlAttribulte(string name, string module, string type)
        {
            Name = name;
            Module = module;
            Type = type;
        }

        public string Name { get; set; }
        public string Module { get; set; }
        public string Type { get; set; } // group . admin/teacher/student
        public HashSet<string> Acts { get; set; } = new HashSet<string>();
    }
    public class IndefindCtrlModel
    {
        public string CtrlName { get; set; }
        public HashSet<string> Acts { get; set; }
    }
    public class IndefindCtrlService
    {
        public IndefindCtrlService()
        {
           
        }
        private readonly List<IndefindCtrlAttribulte> _admin;
        private readonly List<IndefindCtrlAttribulte> _student;
        private readonly List<IndefindCtrlAttribulte> _teacher;

        public List<IndefindCtrlAttribulte> getByAdmin()
        {
            string key = Config.CacheCtrlAdmin;
            var cusMenus = new List<IndefindCtrlAttribulte>();

            var cacheMenu = CacheExtends.GetDataFromCache<List<IndefindCtrlAttribulte>>(key);
            if (cacheMenu != null) return cacheMenu;
            //nếu có
            var itemxx = Assembly.GetAssembly(typeof(AdminController)).GetTypes().ToList();
            var typeAssembly = typeof(AdminController).FullName;
            var assembly = itemxx
                .Where(o=>o.BaseType != null)
                .Where(o => o.BaseType.FullName == typeAssembly).ToList();

            int count = assembly != null ? assembly.Count : 0;
            for (int i = 0; i < count; i++)
            {
                var item = assembly[i];
                var attribute = item.GetCustomAttribute<IndefindCtrlAttribulte>();
                if (attribute == null) continue;
                var getmetho = item.GetMethods();
                var method = getmetho
                    .Where(o=>o.Module != null)
                    .Where(o => o.Module.Name == typeof(AdminController).Module.Name)
                    .Select(o => o.Name).Distinct();
                if (method != null)
                {
                    attribute.Acts = method as HashSet<string>;
                }
                cusMenus.Add(attribute);
            }
            CacheExtends.SetObjectFromCache(key, 360 * 24 * 60, cusMenus);

            return cusMenus;
            
        }
    }
    //    private HashSet<IndefindCtrlModel> getListByType(string type)
    //    {
    //        var key = "";
    //        switch (type)
    //        {
    //            case "teacher": key = Config.CacheCtrlTeacher; break;
    //            case "admin": ; break;
    //            case "student": key = Config.CacheCtrlStudent; break;
    //            default: return null;
    //        }
    //        var cacheMenu = CacheExtends.GetDataFromCache<HashSet<IndefindCtrlModel>>(key);
    //        if (cacheMenu != null) return cacheMenu;
    //        else
    //        {
    //            var cusMenus = new HashSet<IndefindCtrlAttribulte>();
    //            var assembly = Assembly.GetAssembly(typeof(AdminController)).GetTypes()
    //                .Where(o => o.BaseType.FullName == typeof(AdminController).FullName)
    //                .ToList();
    //            int count = assembly != null ? assembly.Count : 0;
    //            for (int i = 0; i < count; i++)
    //            {
    //                var item = assembly[i];
    //                var attribute = item.GetCustomAttribute<MenuControlAttribute>();
    //                if (attribute == null) continue;
    //                var getmetho = item.GetMethods();
    //                var method = getmetho
    //                    .Where(o => o.Module.Name == "BaseMVC.dll")
    //                    .Select(o => o.Name).ToList();
    //                if (method != null)
    //                {
    //                    foreach (var m in method)
    //                    {
    //                        attribute.ActionName.Add(m.ToLower());
    //                    }
    //                }
    //                attribute.Color = string.IsNullOrEmpty(attribute.Color) ? new GetColor().RandomColor() : attribute.Color;
    //                cusMenus.Add(attribute);
    //            }
    //            CacheExtends.SetObjectFromCache(CacheExtends.DefaultAdminController, 360 * 24 * 60, cusMenus);

    //            return cusMenus;
    //        }
    //    private HashSet<IndefindCtrlAttribulte> getAll()
    //    {

    //    }
    //}
}
