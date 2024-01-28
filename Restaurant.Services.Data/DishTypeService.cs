namespace Restaurant.Services.Data
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;
	public class DishTypeService : IDishTypeService
	{
		private readonly RestaurantDbContext context;

		public DishTypeService(RestaurantDbContext _context)
		{
			context = _context;
		}

		public async Task AddDishTypeAsync(AddDishTypeViewModel model)
		{
			if (!context.DishTypes.Where(t => t.IsDeleted == false).Any(t => t.Name == model.Name))
			{
				Restaurant.Data.Models.DishType dishType = new Restaurant.Data.Models.DishType()
				{
					Name = model.Name
				};

				await context.DishTypes.AddAsync(dishType);
				await context.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException("Already existing type with that name.");
			}
		}

		public async Task<IEnumerable<AllDishTypesViewModel>> AllDishTypesAsync()
		{
			return await context.DishTypes.Where(dt => dt.IsDeleted == false)
				.Select(dt => new AllDishTypesViewModel()
				{
					Id = dt.Id,
					Name = dt.Name
				}).ToListAsync();
		}

		public async Task DeleteDishTypeAsync(int typeId)
		{
			var dishType = await context.DishTypes.Where(dt => dt.Id == typeId && dt.IsDeleted == false).FirstOrDefaultAsync();

			if (dishType != null)
			{
				dishType.IsDeleted = true;
				await context.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException("Invalid dish type Id");
			}
		}

		public async Task EditDishTypeById(AddDishTypeViewModel model, int id)
		{
			Restaurant.Data.Models.DishType? dishType = await context.DishTypes.FindAsync(id);

			if (dishType == null)
			{
				throw new ArgumentException("Invalid dish type id.");
			}

			dishType.Name = model.Name;

			await context.SaveChangesAsync();
		}

		public async Task<AddDishTypeViewModel?> GetDishTypeForEditById(int id)
		{
			return await context.DishTypes.Where(dt => dt.Id == id).Select(dt => new AddDishTypeViewModel()
			{
				Name = dt.Name
			}).FirstOrDefaultAsync();
		}
	}
}
