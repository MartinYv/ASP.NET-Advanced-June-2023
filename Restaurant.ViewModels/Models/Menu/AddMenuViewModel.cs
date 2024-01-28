namespace Restaurant.ViewModels.Models.Menu
{
	using Restaurant.Data.Models;
	using System.ComponentModel.DataAnnotations;

	using static Restaurant.Common.EntityValidationConstants.Menu;

	public class AddMenuViewModel
	{
		public int Id { get; set; }

		[Required]
		[StringLength(MenuUrlMaxLength, MinimumLength = MenuUrlMinLength)]
		public string ImageUrl { get; set; } = null!;
		public IEnumerable<DishType> MenuTypes { get; set; } = new List<DishType>();
	}
}