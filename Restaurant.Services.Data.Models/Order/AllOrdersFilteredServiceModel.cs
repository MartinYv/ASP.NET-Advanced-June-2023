namespace Restaurant.Services.Data.Models.Order
{
	using Restaurant.ViewModels.Models.Order;

	public class AllOrdersFilteredServiceModel
	{
		public int TotalOrdersCount { get; set; }
		public IEnumerable<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
	}
}