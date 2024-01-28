namespace Restaurant.Data.Models
{
	using Microsoft.EntityFrameworkCore;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	public class CartDetail
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Cart")]
		public int ShoppingCartId { get; set; }
		public ShoppingCart ShoppingCart { get; set; } = null!;


		[ForeignKey("Dish")]
		public int DishId { get; set; }
		public Dish Dish { get; set; } = null!;


		[Required]
		public int Quantity { get; set; }

		[Required]
		[Precision(18, 2)]
		public decimal UnitPrice { get; set; }
	}
}
