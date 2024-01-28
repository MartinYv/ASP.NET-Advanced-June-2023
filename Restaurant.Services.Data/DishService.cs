namespace Restaurant.Services.Data
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;

	public class DishService : IDishService
	{
		private readonly RestaurantDbContext context;
		private readonly IMenuService menuService;

		public DishService(RestaurantDbContext _context,
			IMenuService menuService)
		{
			context = _context;
			this.menuService = menuService;
		}

		public async Task Add(AddDishViewModel model)
		{
			var dishTypeName = await context.DishTypes.Where(dt => dt.IsDeleted == false && dt.Id == model.DishTypeId).FirstOrDefaultAsync();

			Dish dish = new Dish()
			{
				Name = model.Name,
				Description = model.Description,
				DishTypeId = model.DishTypeId,
				DishType = dishTypeName!,
				ImageUrl = model.ImageUrl,
				Price = model.Price,
				IsDeleted = false
			};

			await menuService.AddDishAsync(dish);
			await context.Dishes.AddAsync(dish);

			await context.SaveChangesAsync();
		}

		public async Task<IEnumerable<AllDishesViewModel>> AllDishesAsync()
		{
			return await context.Dishes.Where(d => d.IsDeleted == false)
				.Select(d => new AllDishesViewModel
				{
					Id = d.Id,
					Name = d.Name,
					Description = d.Description,
					ImageUrl = d.ImageUrl,
					Price = d.Price,
					DishType = d.DishType.Name
				}).ToListAsync();
		}

		public async Task<IEnumerable<DishType>> AllDishTypesAsync()
		{
			return await context.DishTypes.Where(dt => dt.IsDeleted == false).ToListAsync();
		}

		public async Task DeleteDishByIdAsync(int id)
		{
			Dish? dish = await context.Dishes.Where(d => d.IsDeleted == false && d.Id == id).FirstOrDefaultAsync();

			if (dish != null)
			{
				dish.IsDeleted = true;
				await context.SaveChangesAsync();
			}
			else
			{
				throw new ArgumentException("Invalid dish Id.");
			}
		}

		public async Task<Dish?> GetDishById(int Id)
		{
			return await context.Dishes.FirstOrDefaultAsync(d => d.IsDeleted == false && d.Id == Id);
		}

		public async Task<AddDishViewModel?> GetDishForEditByIdAsync(int id)
		{
			Dish? dish = await GetDishById(id);

			if (dish == null)
			{
				throw new ArgumentException("Invalid dish id.");
			}

			AddDishViewModel model = new AddDishViewModel()
			{
				Name = dish.Name,
				Description = dish.Description,
				ImageUrl = dish.ImageUrl,
				Price = dish.Price,
				DishTypeId = dish.Id,
				DishTypes = await AllDishTypesAsync()
			};

			return model;
		}

		public async Task EditDishById(AddDishViewModel modelForEdit, int id)
		{
			Dish? dish = await GetDishById(id);

			if (dish == null)
			{
				throw new ArgumentException("Cannot find dish with that id.");
			}

			dish.Description = modelForEdit.Description;
			dish.Name = modelForEdit.Name;
			dish.ImageUrl = modelForEdit.ImageUrl;
			dish.Price = modelForEdit.Price;
			dish.DishTypeId = modelForEdit.DishTypeId;

			await context.SaveChangesAsync();
		}
	}
}