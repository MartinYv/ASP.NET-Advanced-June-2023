namespace Restaurant.ViewModels.Models.Dish
{
	using Microsoft.EntityFrameworkCore;
	using System.ComponentModel.DataAnnotations;

	using Restaurant.Data.Models;

	using static Restaurant.Common.EntityValidationConstants.Dish;
	public class AddDishViewModel
	{
		public int Id { get; set; }
		[Required]
		[StringLength(DishNameMaxLength, MinimumLength = DishNameMinLength)]
		public string Name { get; set; } = null!;

		[Required]
		[StringLength(DishDescriptionMaxLength, MinimumLength = DishDescriptionMinLength)]
		public string Description { get; set; } = null!;

		[Required]
		[MaxLength(DishUrlMaxLength)]
		[Url]
		public string ImageUrl { get; set; } = null!;


		[Required]
		[Precision(18, 2)]
        [Range(DishPriceMinLength, DishPriceMaxLength)]
        public decimal Price { get; set; }

		public int DishTypeId { get; set; }
		public IEnumerable<DishType> DishTypes { get; set; } = new List<DishType>();
	}
}