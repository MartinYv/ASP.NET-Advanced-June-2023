namespace Restaurant.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Restaurant.Services.Data.Interfaces;
	using Restaurant.Services.Data.Models.Menu;
	using Restaurant.ViewModels.Models.Menu;

	using static Restaurant.Common.NotificationMessagesConstants;
	using static Restaurant.Common.GeneralApplicationConstants;


	[Authorize(Roles = AdminRoleName)]
	public class MenuController : Controller
	{
		private readonly IMenuService menuService;

		public MenuController(IMenuService _menuService)
		{
			menuService = _menuService;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			AddMenuViewModel model = new AddMenuViewModel()
			{
				MenuTypes = await menuService.GetAllMenuTypesAsync()
			};

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Add(AddMenuViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await menuService.AddMenuAcync(model);

				TempData[SuccessMessage] = "Menu succecfully added.";
				return RedirectToAction(nameof(All));
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
				return RedirectToAction(nameof(Add));
			}
		}

		[AllowAnonymous]
		public async Task<IActionResult> All()
		{
			var model = await menuService.AllMenusAsync();
			return View(model);
		}

		public async Task<IActionResult> Delete(int menuId)
		{
			try
			{
				await menuService.DeleteMenuAsync(menuId);

				TempData[SuccessMessage] = "Menu succecfully deleted.";
				return RedirectToAction(nameof(All));
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
				return RedirectToAction(nameof(All));
			}
		}

		[AllowAnonymous]
		public async Task<IActionResult> AllMenuDishes([FromQuery] AllMenuDishesQueryViewModel queryModel, int menuId)
		{
			try
			{
				queryModel.MenuId = menuId;

				AllMenuDishesFilteredServiceModel serviceModel =
				await menuService.MenuAllDishesAsync(queryModel);

				queryModel.Dishes = serviceModel.Dishes;
				queryModel.TotalDishes = serviceModel.TotalDishesCount;

				return View("AllMenuDishesSorted", queryModel);
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}
		}
	}
}