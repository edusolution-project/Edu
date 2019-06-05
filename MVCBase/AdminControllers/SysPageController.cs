using MVCBase.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace MVCBase.AdminControllers
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
