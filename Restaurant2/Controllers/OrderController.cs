namespace Restaurant.Web.Controllers
{
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	using Restaurant.Services.Data.Interfaces;
	using Restaurant.Services.Data.Models.Order;
	using Restaurant.ViewModels.Models.Order;

	using static Restaurant.Common.NotificationMessagesConstants;
	using static Restaurant.Common.GeneralApplicationConstants;


	[Authorize]
	public class OrderController : Controller
	{
		private readonly IOrderService orderService;

		public OrderController(IOrderService _orderService)
		{
			orderService = _orderService;
		}

		public async Task<IActionResult> All()
		{
			var model = await orderService.AllOrdersAcync();
			return View(model);
		}

		[HttpGet]
		public IActionResult FinishOrder()
		{
			OrderUsersInfoViewModel usersInfo = new OrderUsersInfoViewModel();
			return View(usersInfo);
		}

		[HttpPost]
		public IActionResult FinishOrder(OrderUsersInfoViewModel usersInfo)
		{
			if (!ModelState.IsValid)
			{
				return View(usersInfo);
			}

			return RedirectToAction("CheckOut", "Cart", usersInfo);
		}

		[HttpGet]
		public async Task<IActionResult> MyOrders([FromQuery] AllOrdersQueryViewModel queryModel)
		{
			try
			{
				AllOrdersFilteredServiceModel serviceModel =
				await orderService.UserOrdersAsync(queryModel);

				queryModel.Orders = serviceModel.Orders;
				queryModel.TotalOrders = serviceModel.TotalOrdersCount;

				return View("Mine", queryModel);
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}
		}

		[HttpGet]
		[Authorize(Roles = AdminRoleName)]
		public async Task<IActionResult> AllFiltered([FromQuery] AllOrdersQueryViewModel queryModel)
		{
			AllOrdersFilteredServiceModel serviceModel =
				await orderService.AllFilteredAsync(queryModel);

			queryModel.Orders = serviceModel.Orders;
			queryModel.TotalOrders = serviceModel.TotalOrdersCount;

			return View("AllSorted", queryModel);
		}


		[HttpPost]
		[Authorize(Roles = AdminRoleName)]
		public async Task<IActionResult> ChangeStatus(int orderId)
		{
			try
			{
				await orderService.ChangeStatusByIdAsync(orderId);
				TempData[SuccessMessage] = "Order status successfully changed.";
			}
			catch (Exception ex)
			{
				TempData[ErrorMessage] = ex.Message;
			}

			return RedirectToAction(nameof(AllFiltered));
		}
	}
}