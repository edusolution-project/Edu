using BaseMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModPrograms",
        Name = "MO : Quản lý chương trình",
        Order = 40,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModProgramsController : AdminController
    {
    }
}
