namespace Restaurant.ViewModels.Models.Order
{
	using Restaurant.Data.Models;
	public class OrderViewModel
	{
		public int Id { get; set; }
		public Guid CustomerId { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public string Phone { get; set; } = null!;
		public string Address { get; set; } = null!;
		public string Price { get; set; } = null!;
		public string? PromoCode { get; set; } = null!;
		public string CreateDate { get; set; } = null!;
		public string IsCompleted { get; set; } = null!;
		public List<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
	}
}