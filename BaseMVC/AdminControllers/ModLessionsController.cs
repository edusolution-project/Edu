using BaseMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BaseMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModLessions",
        Name = "MO : Quản lý bài học",
        Order = 40,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModLessionsController : AdminController
    {
    }
}
