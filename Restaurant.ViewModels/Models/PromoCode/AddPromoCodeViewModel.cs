namespace Restaurant.ViewModels.Models.PromoCode
{
	using System.ComponentModel.DataAnnotations;

	public class AddPromoCodeViewModel
	{
		[Range(1, int.MaxValue, ErrorMessage = "The maximum usage times cannot be less than 0!")]
		[Display(Name = "How many times it can be used")]
		[Required]
		public int MaxUsageTimes { get; set; }

		[Required]
		public DateTime ExpirationDate { get; set; }

		[Range(0, 100, ErrorMessage = "The promotional percent must be between 0 and 100!")]
		[Required]
		[Display(Name = "Discount percent")]
		public int PromoPercent { get; set; }
	}
}