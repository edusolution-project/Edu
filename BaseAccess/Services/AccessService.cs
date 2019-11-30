using BaseAccess.Attribule;
using BaseAccess.Interfaces;
using BaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;

namespace BaseAccess.Services
{
    public class AccessService : IAccess 
    {
        private const string _claimType = "Permission";
        public AccessService()
        {

        }
        public string ClaimType => _claimType;
        /// <summary>
        /// lấy danh sách theo attribue => dùng làm menu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group"></param>
        /// <returns></returns>
        public List<AccessCtrlAttribute> GetAccessByAttribue<T>(Assembly assembly, string group)
        {
            var listItem = new List<AccessCtrlAttribute>();
            var assemb = getAssemblyType(assembly);
            var typeAssembly = typeof(T).FullName;
            var listController = assemb
                .Where(type => typeof(T).IsAssignableFrom(type))
                .Where(o => o.BaseType.FullName == typeAssembly)?
                .ToList();
            int count = listController != null ? listController.Count : 0;
            for (int i = 0; i < count; i++)
            {
                var item = listController[i];
                string module = item.Name.Replace("Controller", "").ToLower();
                var attribute = item.GetCustomAttribute<AccessCtrlAttribute>();
                if (attribute == null)
                {
                    attribute = new AccessCtrlAttribute(module, module, group);
                }
                else
                {
                    attribute.Module = module;
                    attribute.Type = group;
                }
                var actions = item.GetTypeInfo().DeclaredMethods?
                        .Where(o => o.IsPublic)
                        .Select(o => o.Name.ToLower());

                if (actions != null)
                {
                    attribute.Acts = new HashSet<string>(actions);
                }
                listItem.Add(attribute);
            }

            return listItem;
        }
        /// <summary>
        /// lấy danh sách AccessModel
        /// </summary>
        /// <typeparam name="T">Controller hoặc Class</typeparam>
        /// <param name="group"> admin/teacher/student/user ... </param>
        /// <returns></returns>
        public List<AccessModel> GetAccessWithoutAttribue<T>(Assembly assembly,string group)
        {
            var listItem = new List<AccessModel>();
            var assemb = getAssemblyType(assembly);
            var typeAssembly = typeof(T).FullName;
            var listController = assemb
                .Where(type => typeof(T).IsAssignableFrom(type))
                .Where(o => o.BaseType.FullName == typeAssembly)?
                .ToList();
            int count = listController != null ? listController.Count : 0;
            for (int i = 0; i < count; i++)
            {
                var item = listController[i];
                string module = item.Name.Replace("Controller", "").ToLower();
                var attribute = new AccessModel()
                {
                    Ctrl = module,
                    Group = group
                };
                var actions = item.GetTypeInfo().DeclaredMethods?
                        .Where(o => o.IsPublic)
                        .Select(o => o.Name.ToLower());

                if (actions != null)
                {
                    attribute.Acts = new HashSet<string>(actions);
                }
                listItem.Add(attribute);
            }

            return listItem;
        }
        /// <summary>
        /// Check quyền
        /// </summary>
        /// <param name="user"> User theo Authencation  </param>
        /// <param name="ctrl"> ControllerName </param>
        /// <param name="action"> ActionName </param>
        /// <param name="type"> admin/(user:{ teacher/student }) </param>
        /// <returns></returns>
        public bool IsAccess(ClaimsPrincipal user, string ctrl, string action, string type)
        {
            if (user.IsInRole("supperadmin") || user.IsInRole("root")) return true;
            string value = type == "admin" ? $"{ctrl}{action}" : ctrl;
            var permissions = user.FindAll(o => o.Type == _claimType && o.Value == value)?.ToList();
            return permissions != null;
        }
        /// <summary>
        /// lấy danh sách assembly excuting
        /// </summary>
        /// <returns></returns>
        private Type[] getAssemblyType(Assembly assembly)
        {
            return assembly.GetTypes();
        }
    }
}
