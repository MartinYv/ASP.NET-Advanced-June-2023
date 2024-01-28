namespace Restaurant.Services.Data
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.Services.Data.Models.Menu;
	using Restaurant.ViewModels.Models.Menu;
	using Restaurant.ViewModels.Models.Dish;
	using Restaurant.ViewModels.Order.Enum;

	public class MenuService : IMenuService
	{
		private readonly RestaurantDbContext context;

		public MenuService(RestaurantDbContext _context)
		{
			context = _context;
		}

		public async Task AddMenuAcync(AddMenuViewModel model)
		{
			bool isMenuExist = context.Menus.Where(m => m.IsDeleted == false).Any(mt => mt.DishType.Id == model.Id);

			if (isMenuExist == true)
			{
				throw new ArgumentException("Menu with that type is already added.");
			}

			Menu menu = new Menu()
			{
				DishTypeId = model.Id,
				ImageUrl = model.ImageUrl
			};

			await context.Menus.AddAsync(menu);
			await context.SaveChangesAsync();
		}

		public async Task<IEnumerable<AllMenusViewModel>> AllMenusAsync()
		{
			return await context.Menus.Where(m => m.IsDeleted == false).Select(m =>
				  new AllMenusViewModel()
				  {
					  Id = m.Id,
					  MenuType = m.DishType.Name,
					  ImageUrl = m.ImageUrl
				  }).ToListAsync();
		}

		public async Task<IEnumerable<DishType>> GetAllMenuTypesAsync()
		{
			return await context.DishTypes.Where(m => m.IsDeleted == false).ToListAsync();
		}

		public async Task DeleteMenuAsync(int menuId)
		{
			var menu = await context.Menus.Where(m => m.IsDeleted == false).FirstOrDefaultAsync(m => m.Id == menuId);

			if (menu == null)
			{
				throw new ArgumentException("Invalid menu Id");
			}

			menu.IsDeleted = true;
			await context.SaveChangesAsync();
		}

		public async Task<Menu?> GetMenuByName(string menuName)
		{
			return await context.Menus.Where(m => m.DishType.Name == menuName && m.IsDeleted == false).Include(x => x.DishType).Include(x => x.Dishes).FirstOrDefaultAsync();
		}

		public async Task AddDishAsync(Dish dish)
		{
			Menu? menu = await GetMenuByName(dish.DishType.Name);

			if (menu == null)
			{
				throw new ArgumentException("There isn't menu with that type of dishes. First add the menu.");
			}

			if (menu.Dishes.Where(d => d.IsDeleted == false).Any(d => d.Name == dish.Name))
			{
				throw new ArgumentException("Dish with that name is already added");
			}

			menu.Dishes.Add(dish);
		}

		public async Task<AllMenuDishesFilteredServiceModel> MenuAllDishesAsync(AllMenuDishesQueryViewModel queryModel)
		{
			var menu = await context.Menus.Where(m => m.Id == queryModel.MenuId && m.IsDeleted == false).Include(d => d.Dishes).FirstOrDefaultAsync();

			IQueryable<Dish> menuQuery = menu!.Dishes.AsQueryable();

			menuQuery = queryModel.OrderSorting switch
			{
				OrderSorting.PriceAscending => menuQuery
					.OrderBy(d => d.Price),
				OrderSorting.PriceDescending => menuQuery
					.OrderByDescending(d => d.Price),
				_ => menuQuery.OrderBy(d => d.Id)
			};

			IEnumerable<DishViewModel> allDishes = menuQuery
		   .Where(d => d.IsDeleted == false)
		   .Skip((queryModel.CurrentPage - 1) * queryModel.DishesPerPage)
		   .Take(queryModel.DishesPerPage)
		   .Select(d => new DishViewModel
		   {
			   Id = d.Id,
			   Description = d.Description,
			   ImageUrl = d.ImageUrl,
			   Name = d.Name,
			   Price = d.Price
		   })
		   .ToList();

			int totalDishes = menuQuery.Count();

			return new AllMenuDishesFilteredServiceModel()
			{
				TotalDishesCount = totalDishes,
				Dishes = allDishes
			};
		}
	}
}