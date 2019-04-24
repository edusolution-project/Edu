using BaseMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseMVC.AdminControllers
{
    [MenuControl(
        CModule = "SysPage",
        Name = "Sys : Quản lý trang",
        Order = 40,
        IShow = true,
        Type = MenuType.Sys
    )]
    public class SysPageController : AdminController
    {
    }
}
