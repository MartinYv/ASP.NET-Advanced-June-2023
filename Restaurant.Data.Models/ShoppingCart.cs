namespace Restaurant.Data.Models
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	public class ShoppingCart
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("User")]
		public Guid UserId { get; set; }
		public ApplicationUser User { get; set; } = null!;

		public bool IsDeleted { get; set; } = false;
		public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
	}
}
