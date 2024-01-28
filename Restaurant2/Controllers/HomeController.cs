namespace Restaurant.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using static Restaurant.Common.GeneralApplicationConstants;

	[AllowAnonymous]
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			if (User.IsInRole(AdminRoleName))
			{
				return RedirectToAction("Index", "Home", new { area = AdminAreaName });
			}
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		public IActionResult Gallery()
		{
			return View("Gallery");
		}
		public IActionResult Menu()
		{
			return View();
		}
		public IActionResult Order()
		{
			return View();
		}
		public IActionResult Reservation()
		{
			return View();
		}
		public IActionResult About()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statusCode)
		{
			if (statusCode == 400 || statusCode == 404 || statusCode == 0)
			{
				return View("Error404");
			}

			if (statusCode == 401)
			{
				return View("Error401");
			}

			return View();
		}
	}
}