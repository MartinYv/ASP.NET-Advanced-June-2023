namespace Restaurant.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.PromoCode;

	using static Restaurant.Common.NotificationMessagesConstants;
	using static Restaurant.Common.GeneralApplicationConstants;

	[Authorize(Roles = AdminRoleName)]
	public class PromoCodeController : Controller
	{
		private readonly IPromoCodeService promoCodeService;

		public PromoCodeController(IPromoCodeService _promoCodeService)
		{
			promoCodeService = _promoCodeService;
		}

		[HttpGet]
		public IActionResult Add()
		{
			var model = new AddPromoCodeViewModel()
			{

			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddPromoCodeViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (model.ExpirationDate < DateTime.Now)
			{
				ModelState.AddModelError("", "The date can not be bellow today's date.");
				return View(model);
			}

			await promoCodeService.AddPromoCodeAsync(model);

			TempData[SuccessMessage] = "Successfully added.";

			return RedirectToAction(nameof(All));
		}

		public async Task<IActionResult> All()
		{
			var model = await promoCodeService.GetAllPromoCodesAsync();
			return View(model);
		}

		public async Task<IActionResult> Delete(int id)
		{
			try
			{
				await promoCodeService.DeletePromoCodeAsync(id);

				TempData[SuccessMessage] = "Successfully deleted.";
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