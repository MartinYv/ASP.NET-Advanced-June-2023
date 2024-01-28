namespace Restaurant.Services.Data.Interfaces
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Restaurant.ViewModels.Models.Dish;
	public interface IDishTypeService
	{
		Task AddDishTypeAsync(AddDishTypeViewModel model);
		Task<IEnumerable<AllDishTypesViewModel>> AllDishTypesAsync();
		Task DeleteDishTypeAsync(int id);
		Task EditDishTypeById(AddDishTypeViewModel model, int id);
		Task<AddDishTypeViewModel?> GetDishTypeForEditById(int id);
	}
}