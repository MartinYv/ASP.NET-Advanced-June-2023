namespace Restaurant.Web.Areas.Admin.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

    using static Restaurant.Common.GeneralApplicationConstants;

	[Area(AdminAreaName)]
    [Authorize(Roles = AdminRoleName)]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}