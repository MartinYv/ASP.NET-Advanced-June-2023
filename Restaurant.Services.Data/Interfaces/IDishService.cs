namespace Restaurant.Services.Data.Interfaces
{
	using Restaurant.Data.Models;
	using Restaurant.ViewModels.Models.Dish;
	public interface IDishService
	{
		Task Add(AddDishViewModel model);
		Task<IEnumerable<DishType>> AllDishTypesAsync();
		Task<IEnumerable<AllDishesViewModel>> AllDishesAsync();
		Task<Dish?> GetDishById(int id);
		Task DeleteDishByIdAsync(int id);
		Task<AddDishViewModel?> GetDishForEditByIdAsync(int id);
		Task EditDishById(AddDishViewModel modelForEdit, int id);
	}
}