namespace Restaurant.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Authorization;

	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;

	using static Restaurant.Common.NotificationMessagesConstants;
	using static Restaurant.Common.GeneralApplicationConstants;


	[Authorize(Roles = AdminRoleName)]
	public class DishTypeController : Controller
    {
        private readonly IDishTypeService dishTypeService;

        public DishTypeController(IDishTypeService _dishTypeService)
        {
            dishTypeService = _dishTypeService;
        }

        [HttpGet]
        public IActionResult Add()
        {
            var model = new AddDishTypeViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddDishTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await dishTypeService.AddDishTypeAsync(model);

                TempData[SuccessMessage] = "Dish type successfully added.";
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
            var model = await dishTypeService.AllDishTypesAsync();

            return View(model);
        }

        public async Task<IActionResult> Delete(int typeId)
        {
            try
            {
                await dishTypeService.DeleteDishTypeAsync(typeId);
               
                TempData[SuccessMessage] = "Dish type successfully deleted.";
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
            AddDishTypeViewModel? model = await dishTypeService.GetDishTypeForEditById(id);

            if (model == null)
            {
                TempData[ErrorMessage] = "Ivalid dish type Id";
                return RedirectToAction(nameof(All));
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(AddDishTypeViewModel model, int id)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await dishTypeService.EditDishTypeById(model, id);
                TempData[SuccessMessage] = "Dish type successfully edited.";
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