
using BaseMongoDB.Database;
using BaseMongoDB.Factory;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace BaseMVC.Globals
{
    public class ChildNodes
    {
        protected static CPMenuService _menuService = Instance.CreateInstanceCPMenu("CPMenus");
        public static List<string> GetChildMenuByRoot(string RootID)
        {
            var data = _menuService.CreateQuery().Find(o => o.ParentID == RootID || o.ID == RootID)?.ToList();
            if (data == null) return new List<string>() {""};
            else
            {
                List<string> listID = data.Select(o => o.ID).ToList();
                return listID;
            }
        }

    }
}
