using BaseMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModSubjects",
        Name = "MO : Quản lý môn học",
        Order = 40,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModSubjectsController : AdminController
    {
    }
}
