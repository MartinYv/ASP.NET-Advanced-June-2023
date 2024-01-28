namespace Restaurant.Data.Models
{
	using Microsoft.EntityFrameworkCore;

	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	public class OrderDetail
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		[Precision(18, 2)]
		public decimal UnitPrice { get; set; }

		[ForeignKey("Order")]
		public int OrderId { get; set; }
		public Order Order { get; set; } = null!;

		[ForeignKey("Dish")]
		public int DishId { get; set; }
		public Dish Dish { get; set; } = null!;
	}
}

