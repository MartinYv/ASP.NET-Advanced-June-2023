namespace Restaurant.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Authorization;

	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;

	using static Restaurant.Common.NotificationMessagesConstants;
	using static Restaurant.Common.GeneralApplicationConstants;

	[Authorize(Roles = AdminRoleName)]
	public class DishController : Controller
	{
		private readonly IDishService dishService;

		public DishController(IDishService _dishService)
		{
			dishService = _dishService;
		}

		[HttpGet]
		public async Task<IActionResult> Add()
		{
			AddDishViewModel model = new AddDishViewModel()
			{
				DishTypes = await dishService.AllDishTypesAsync()
			};

			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Add(AddDishViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await dishService.Add(model);

				TempData[SuccessMessage] = "Dish successfully added.";
				return RedirectToAction(nameof(All));
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
				return RedirectToAction(nameof(All));
			}
		}

		public async Task<IActionResult> All()
		{
			var model = await dishService.AllDishesAsync();
			return View(model);
		}

		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await dishService.DeleteDishByIdAsync(id);
				TempData[SuccessMessage] = "Dish successfully deleted.";
				return RedirectToAction(nameof(All));
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
				return RedirectToAction(nameof(All));
			}
		}


		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			var model = await dishService.GetDishForEditByIdAsync(id);
			return View(model);
		}


		[HttpPost]
		public async Task<IActionResult> Edit(AddDishViewModel model, int id)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			try
			{
				await dishService.EditDishById(model, id);

				TempData[SuccessMessage] = "Dish successfully deleted.";
				return RedirectToAction(nameof(All));
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
				return RedirectToAction(nameof(All));
			}
		}
	}
}