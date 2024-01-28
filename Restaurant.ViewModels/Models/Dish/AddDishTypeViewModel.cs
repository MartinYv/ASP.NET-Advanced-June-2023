namespace Restaurant.ViewModels.Models.Dish
{
	using System.ComponentModel.DataAnnotations;

	using static Restaurant.Common.EntityValidationConstants.DishType;
	public class AddDishTypeViewModel
	{
		[Required]
		[StringLength(DishTypeMaxLenght, MinimumLength = DishTypeMinLength)]
		public string Name { get; set; } = null!;
	}
}