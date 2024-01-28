namespace Restaurant.Data.Models
{
	using System.ComponentModel.DataAnnotations;

	using static Restaurant.Common.EntityValidationConstants.PromoCode;
	public class PromoCode
	{
		[Required]
		public int Id { get; set; }

		[Required]
		[MaxLength(CodeMaxLength)]
		public string Code { get; set; } = null!;

		[Range(0, int.MaxValue)]
		public int MaxUsageTimes { get; set; }

		[Required]
		public DateTime ExpirationDate { get; set; }

		[Range(0, int.MaxValue)]
		public int UsedTimes { get; set; }
		
		[Range(0, 10)]
		public int PromoPercent { get; set; }
		public bool IsDeleted { get; set; }
	}
}
