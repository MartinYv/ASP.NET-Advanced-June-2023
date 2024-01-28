namespace Restaurant.Data.Models
{
	using Microsoft.EntityFrameworkCore;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;

	using static Restaurant.Common.EntityValidationConstants.Order;
	public class Order
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(FirstNameMaxLength)]
		public string FirstName { get; set; } = null!;

		[Required]
		[MaxLength(LastNameMaxLength)]
		public string LastName { get; set; } = null!;

		[Required]
		[MaxLength(PhoneMaxLength)]
		[Phone]
		public string Phone { get; set; } = null!;

		[Required]
		[MaxLength(AddressMaxLength)]
		public string Address { get; set; } = null!;

		[Required]
		[Precision(18, 2)]
		public decimal Price { get; set; }

		[Required]
		public DateTime CreateDate { get; set; }

		[ForeignKey("PromoCode")]
		public int? PromoCodeId { get; set; }
		public PromoCode? PromoCode { get; set; }
		public bool IsDeleted { get; set; }

		[Required]
		public bool IsCompleted { get; set; }

		[ForeignKey("Customer")]
		public Guid CustomerId { get; set; }
		public ApplicationUser Customer { get; set; } = null!;

		public List<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();
	}
}