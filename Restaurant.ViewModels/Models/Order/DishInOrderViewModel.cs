namespace Restaurant.ViewModels.Models.Order
{
	using Restaurant.Data.Models;
	public class DishInOrderViewModel
	{
		public string Name { get; set; } = null!;
		public string ImageUrl { get; set; } = null!;
		public decimal Price { get; set; }
		public DishType DishType { get; set; } = null!;
		public List<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
		public List<CartDetail> CartDetail { get; set; } = new List<CartDetail>();
	}
}