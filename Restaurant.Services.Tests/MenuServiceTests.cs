namespace Restaurant.Services.Tests
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Menu;

	[TestFixture]
	public class MenuServiceTests
	{
		private RestaurantDbContext dbContext;
		private IMenuService menuService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<RestaurantDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			dbContext = new RestaurantDbContext(options);
			menuService = new MenuService(dbContext);
		}

		[Test]
		public async Task AddMenuAsync_ShouldAddMenu()
		{
			var dishType = new DishType { Name = "Test Dish Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var model = new AddMenuViewModel { Id = dishType.Id, ImageUrl = "test.jpg" };

			await menuService.AddMenuAcync(model);

			var addedMenu = await dbContext.Menus.FirstOrDefaultAsync(m => m.DishTypeId == dishType.Id);
			Assert.NotNull(addedMenu);

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task AddMenuAsync_ShouldThrowArgumentException_WhenMenuExists()
		{
			var dishType = new DishType { Name = "Existing Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var menu = new Menu { DishTypeId = dishType.Id, ImageUrl = "testUrl" };

			await dbContext.Menus.AddAsync(menu);
			await dbContext.SaveChangesAsync();

			var model = new AddMenuViewModel { Id = dishType.Id, ImageUrl = "testUrl" };

			Assert.ThrowsAsync<ArgumentException>(async () => await menuService.AddMenuAcync(model));

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task AddDishAsync_ShouldThrowException_WhenMenuWithDishTypeDoesNotExist()
		{
			var dishType = new DishType { Name = "Test Dish Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var dish = new Dish { Name = "Test Dish", Description = "Description", Price = 10.0M, DishTypeId = dishType.Id, DishType = dishType };

			var exception = Assert.ThrowsAsync<ArgumentException>(async () => await menuService.AddDishAsync(dish));
			Assert.That(exception.Message, Is.EqualTo("There isn't menu with that type of dishes. First add the menu."));

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task AddDishAsync_ShouldAddDishToMenu()
		{
			var dishType = new DishType { Name = "Test Dish Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var menu = new Menu { DishTypeId = dishType.Id, ImageUrl = "menu.jpg" };

			await dbContext.Menus.AddAsync(menu);
			await dbContext.SaveChangesAsync();

			var dish = new Dish { Name = "Test Dish", Description = "Description", Price = 10.0M, DishTypeId = dishType.Id, DishType = dishType };

			await menuService.AddDishAsync(dish);

			menu = await dbContext.Menus.Include(m => m.Dishes).FirstOrDefaultAsync(m => m.DishTypeId == dishType.Id);

			Assert.NotNull(menu);
			Assert.IsTrue(menu.Dishes.Any(d => d.Name == "Test Dish"));

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task AddDishAsync_ShouldThrowException_WhenDishWithSameNameExists()
		{
			var dishType = new DishType { Name = "Test Dish Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var menu = new Menu()
			{
				DishTypeId = dishType.Id,
				DishType = dishType,
				ImageUrl = "Testurl.jpg"
			};

			await dbContext.Menus.AddAsync(menu);
			await dbContext.SaveChangesAsync();

			var existingDish = new Dish()
			{
				Name = "Test Dish",
				Description = "Existing Description",
				ImageUrl = "Testurl.jpg",
				Price = 15.0M,
				DishType = dishType,
				DishTypeId = dishType.Id
			};

			menu.Dishes.Add(existingDish);
			await dbContext.SaveChangesAsync();

			var dish = new Dish()
			{
				Name = "Test Dish",
				Description = "New Description",
				Price = 10.0M,
				DishType = dishType,
				DishTypeId = dishType.Id,
				ImageUrl = "Testurl.jpg"
			};

			var exception = Assert.ThrowsAsync<ArgumentException>(async () => await menuService.AddDishAsync(dish));
			Assert.That(exception.Message, Is.EqualTo("Dish with that name is already added"));

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task GetAllMenuTypes_ShouldReturnAllMenuTypes()
		{
			var dishTypes = new List<DishType>()
			{
				new DishType
				{
					Name = "Dish type 1"
				},
				new DishType
				{
					Name = "Dish type 2"
				}
			};

			await dbContext.AddRangeAsync(dishTypes);
			await dbContext.SaveChangesAsync();

			var result = await menuService.GetAllMenuTypesAsync();

			Assert.That(result.Count(), Is.EqualTo(dishTypes.Count()));
			CollectionAssert.AreEquivalent(result.Select(dt => dt.Name), result.Select(dt => dt.Name));
		}

		[Test]
		public async Task AllMenusAsync_ShouldReturnAllMenus()
		{
			var dishTypes = new List<DishType>
			{
				new DishType { Name = "Type 1" },
				new DishType { Name = "Type 2" },
				new DishType { Name = "Type 3" }
			};

			await dbContext.DishTypes.AddRangeAsync(dishTypes);
			await dbContext.SaveChangesAsync();

			var menus = new List<Menu>
			{
				new Menu { DishTypeId = dishTypes[0].Id, ImageUrl = "menu1.jpg" },
				new Menu { DishTypeId = dishTypes[1].Id, ImageUrl = "menu2.jpg" },
				new Menu { DishTypeId = dishTypes[2].Id, ImageUrl = "menu3.jpg" }
			};

			await dbContext.Menus.AddRangeAsync(menus);
			await dbContext.SaveChangesAsync();

			var result = await menuService.AllMenusAsync();

			Assert.That(result.Count(), Is.EqualTo(3));
			CollectionAssert.AreEquivalent(menus.Select(m => m.ImageUrl), result.Select(m => m.ImageUrl));

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public async Task DeleteMenuAsync_ShouldDeleteMenu()
		{
			var menu = new Menu { DishTypeId = 1, ImageUrl = "imgUrl" };

			await dbContext.Menus.AddAsync(menu);
			await dbContext.SaveChangesAsync();

			await menuService.DeleteMenuAsync(menu.Id);

			var deletedMenu = await dbContext.Menus.FindAsync(menu.Id);
			Assert.True(deletedMenu!.IsDeleted);

			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public void DeleteMenuAsync_ShouldThrowArgumentException_WhenInvalidMenuId()
		{
			var invalidMenuId = -1;

			Assert.ThrowsAsync<ArgumentException>(async () => await menuService.DeleteMenuAsync(invalidMenuId));
		}

		[Test]
		public async Task GetMenuByName_ShouldReturnCorrectMenu()
		{
			var dishType = new DishType { Name = "Test Dish Type" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var menu = new Menu { DishTypeId = dishType.Id, ImageUrl = "menu.jpg" };

			await dbContext.Menus.AddAsync(menu);
			await dbContext.SaveChangesAsync();

			var result = await menuService.GetMenuByName(dishType.Name);

			Assert.That(result, Is.Not.Null);
			Assert.That(dishType.Id, Is.EqualTo(result.DishTypeId));

			dbContext.Database.EnsureDeleted();
		}


		[Test]
		public async Task GetMenuByName_ShouldReturnNullWhenMenuDoesntExist()
		{
			string invalidDishType = "InvalidDishTypeName";
			var result = await menuService.GetMenuByName(invalidDishType);

			Assert.That(result, Is.Null);
		}
	}
}