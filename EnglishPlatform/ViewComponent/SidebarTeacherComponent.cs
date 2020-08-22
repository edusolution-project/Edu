using BaseCustomerEntity.Database;
using BaseCustomerMVC.Globals;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class SidebarTeacherViewComponent : ViewComponent
{
    private readonly CenterService _centerService;
    private readonly TeacherHelper _teacherHelper;

    public SidebarTeacherViewComponent(CenterService centerService, TeacherHelper teacherHelper)
    {
        _teacherHelper = teacherHelper;
        _centerService = centerService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string centerCode)
    {
        string userID = UserClaimsPrincipal.FindFirst("UserID")?.Value;
        var center = ViewBag.Center as CenterEntity;
        if (center == null)
        {
            center = _centerService.GetItemByCode(centerCode);
            ViewBag.Center = center;
        }
        if (ViewBag.IsHeadTeacher == null)
            ViewBag.IsHeadTeacher = _teacherHelper.HasRole(userID, center.ID, "head-teacher");
        return View(center);
    }
}
