namespace Restaurant.Services.Data.Interfaces
{
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Models.Menu;
	using Restaurant.ViewModels.Models.Menu;
	public interface IMenuService
	{
		Task AddMenuAcync(AddMenuViewModel model);
		Task<IEnumerable<DishType>> GetAllMenuTypesAsync();
		Task<IEnumerable<AllMenusViewModel>> AllMenusAsync();
		Task DeleteMenuAsync(int menuId);
		Task<Menu?> GetMenuByName(string menuName);
		Task AddDishAsync(Dish dish);
		Task<AllMenuDishesFilteredServiceModel> MenuAllDishesAsync(AllMenuDishesQueryViewModel queryModel);
	}
}