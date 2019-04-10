using BaseModels;
using System.Collections.Generic;
using System.Linq;

namespace MVCBase.Globals
{
    public class ChildNodes
    {
        protected static CPMenuService _menuService = new CPMenuService();
        public static List<int> GetChildMenuByRoot(int RootID)
        {
            var data = _menuService.CreateQuery().Find(o => o.ParentID == RootID || o.ID == RootID).ToList();
            if (data == null) return new List<int>() {0};
            else
            {
                List<int> listID = data.Select(o => o.ID).ToList();
                return listID;
            }
        }

    }
}
