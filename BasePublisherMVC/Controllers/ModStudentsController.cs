using BasePublisherMVC.Globals;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasePublisherMVC.AdminControllers
{
    [MenuControl(
        CModule = "ModStudents",
        Name = "MO : Quản lý học viên",
        Order = 40,
        IShow = true,
        Type = MenuType.Mod
    )]
    public class ModStudentsController : AdminController
    {
    }
}
