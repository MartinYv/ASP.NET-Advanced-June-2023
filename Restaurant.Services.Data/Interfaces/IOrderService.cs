namespace Restaurant.Services.Data.Interfaces
{
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Models.Order;
	using Restaurant.ViewModels.Models.Order;
	public interface IOrderService
	{
		Task<IEnumerable<OrderViewModel>> AllOrdersAcync();
		Task<AllOrdersFilteredServiceModel> UserOrdersAsync(AllOrdersQueryViewModel queryModel);
		string? GetUserId();
		Task<AllOrdersFilteredServiceModel> AllFilteredAsync(AllOrdersQueryViewModel queryModel);
		Task<Order?> FindOrderByIdAsync(int orderId);
		Task ChangeStatusByIdAsync(int orderId);
	}
}
