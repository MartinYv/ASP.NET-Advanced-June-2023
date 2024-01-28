namespace Restaurant.Services.Tests
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Services.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;
	using Restaurant.ViewModels.Models.Menu;

	[TestFixture]
	public class DishServiceTests
	{
		private RestaurantDbContext dbContext;
		private IDishTypeService dishTypeService;
		private IDishService dishService;
		private IMenuService menuService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<RestaurantDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			dbContext = new RestaurantDbContext(options);

			menuService = new MenuService(dbContext);
			dishTypeService = new DishTypeService(dbContext);
			dishService = new DishService(dbContext, menuService);
		}

		[Test]
		public async Task Add_ShouldAddDish()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);

			var addedDish = await dbContext.Dishes.FirstOrDefaultAsync(d => d.Name == "Test Dish");

			Assert.NotNull(addedDish);
			Assert.That(addedDish.DishType.Name, Is.EqualTo(dishType.Name));
		}

		[Test]
		public async Task Add_ShouldThrowArgumentException_WhenAddingExistingDish()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);

			Assert.ThrowsAsync<ArgumentException>(async () => await dishService.Add(model));
		}

		[Test]
		public async Task AllDishesAsync_ShouldReturnAllDishes()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};
			var model2 = new AddDishViewModel
			{
				Name = "Test Dish2",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};
			var model3 = new AddDishViewModel
			{
				Name = "Test Dish3",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);
			await dishService.Add(model2);
			await dishService.Add(model3);

			var result = await dishService.AllDishesAsync();

			Assert.That(result.Count(), Is.EqualTo(3));
			CollectionAssert.AreEquivalent(new[] { model.Name, model2.Name, model3.Name }, result.Select(d => d.Name));
		}

		[Test]
		public async Task DeleteDishByIdAsync_ShouldDeleteDish()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);

			await dishService.DeleteDishByIdAsync(model.Id);

			var dish = await dbContext.Dishes.FindAsync(1);

			Assert.True(dish!.IsDeleted);
		}

		[Test]
		public void DeleteDishByIdAsyncShouldThrowArgumentExceptionWhenInvalidDishId()
		{
			var invalidDishId = -1;

			Assert.ThrowsAsync<ArgumentException>(async () => await dishService.DeleteDishByIdAsync(invalidDishId));
		}

		[Test]
		public async Task AllDishTypes_ShouldReturnAllDishTypes()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddDishTypeViewModel dishType2 = new AddDishTypeViewModel() { Name = "Test type2" };
			await dishTypeService.AddDishTypeAsync(dishType2);

			AddDishTypeViewModel dishType3 = new AddDishTypeViewModel() { Name = "Test type3" };
			await dishTypeService.AddDishTypeAsync(dishType3);

			var result = await dbContext.DishTypes.ToArrayAsync();

			Assert.That(result.Count(), Is.EqualTo(3));
			CollectionAssert.AreEquivalent(new[] { dishType.Name, dishType2.Name, dishType3.Name }, result.Select(d => d.Name));
		}

		[Test]
		public async Task GetDishByIdAsync_ShouldReturnValidDish()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);

			var dish = dishService.GetDishById(model.Id);

			Assert.That(model.Id, Is.EqualTo(dish.Id));
		}

		[Test]
		public async Task GetDishByIdAsync_ShouldReturnNull()
		{
			int invalidDishId = 5;

			var dish = await dishService.GetDishById(invalidDishId);

			Assert.That(dish, Is.EqualTo(null));
		}

		[Test]
		public async Task GetDishForEditByIdAsync_ShouldReturnValidModel()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var model = new AddDishViewModel
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dishService.Add(model);

			var resultModel = await dishService.GetDishForEditByIdAsync(model.Id);

			Assert.That(resultModel!.Price, Is.EqualTo(model.Price));
			Assert.That(resultModel!.Description, Is.EqualTo(model.Description));
			Assert.That(resultModel!.DishTypeId, Is.EqualTo(model.DishTypeId));
			Assert.That(resultModel!.Name, Is.EqualTo(model.Name));
		}
		[Test]
		public void GetDishForEditByIdAsync_ShouldThrowExceptionWhenDishIsNull()
		{
			int invalidDishId = 5;

			Assert.ThrowsAsync<ArgumentException>(async () => await dishService.GetDishForEditByIdAsync(invalidDishId));
		}

		public void EditDishById_ShouldThrowExceptionWhenDishIsNull()
		{
			int invalidDishId = 5;

			var model = new AddDishViewModel
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			Assert.ThrowsAsync<ArgumentException>(async () => await dishService.EditDishById(model, invalidDishId));
		}

		[Test]
		public async Task EditDishById_ShouldEditSuccessfull()
		{
			AddDishTypeViewModel dishType = new AddDishTypeViewModel() { Name = "Test type" };
			await dishTypeService.AddDishTypeAsync(dishType);

			AddMenuViewModel menuType = new AddMenuViewModel() { Id = 1, ImageUrl = " TestUrl.bg" };
			await menuService.AddMenuAcync(menuType);

			var dish = new AddDishViewModel
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};
			await dishService.Add(dish);

			var edditedDish = new AddDishViewModel
			{
				Id = 1,
				Name = "Edit",
				Description = "Edit",
				DishTypeId = 1,
				ImageUrl = "Edit.jpg",
				Price = 11m

			};

			await dishService.EditDishById(edditedDish, dish.Id);

			var resultDish = await dishService.GetDishById(dish.Id);

			Assert.That(resultDish!.Price, Is.EqualTo(edditedDish.Price));
			Assert.That(resultDish!.Description, Is.EqualTo(edditedDish.Description));
			Assert.That(resultDish!.DishTypeId, Is.EqualTo(edditedDish.DishTypeId));
			Assert.That(resultDish!.Name, Is.EqualTo(edditedDish.Name));
		}

		[TearDown]
		public void CleanUp()
		{
			dbContext.Database.EnsureDeleted();
			dbContext.Dispose();
		}
	}
}