namespace Restaurant.Services.Data.Models.Menu
{
	using Restaurant.ViewModels.Models.Dish;

	public class AllMenuDishesFilteredServiceModel
	{
		public int TotalDishesCount { get; set; }
		public IEnumerable<DishViewModel> Dishes { get; set; } = new List<DishViewModel>();
	}
}