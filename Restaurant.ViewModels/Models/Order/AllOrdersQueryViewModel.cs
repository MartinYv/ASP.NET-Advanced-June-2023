namespace Restaurant.ViewModels.Models.Order
{
	using System.ComponentModel.DataAnnotations;

	using Restaurant.ViewModels.Order.Enum;

	public class AllOrdersQueryViewModel
	{
		public AllOrdersQueryViewModel()
		{
			CurrentPage = 1;
			OrdersPerPage = 3;
		}

		[Display(Name = "Sort Orders By")]
		public OrderSorting OrderSorting { get; set; }

		public int CurrentPage { get; set; }

		[Display(Name = "Show Orders On Page")]
		public int OrdersPerPage { get; set; }

		public int TotalOrders { get; set; }

		public IEnumerable<OrderViewModel> Orders { get; set; } = new HashSet<OrderViewModel>();
	}
}