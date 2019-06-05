using BasePublisherMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModUnits",
        Name = "MO : Quản lý đơn vị học",
        Order = 40,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModUnitsController : AdminController
    {
    }
}
