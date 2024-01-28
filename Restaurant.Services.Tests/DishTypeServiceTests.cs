namespace Restaurant.Services.Tests
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Dish;

	[TestFixture]
	public class DishTypeServiceTests
	{
		private RestaurantDbContext dbContext;
		private IDishTypeService dishTypeService;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<RestaurantDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			dbContext = new RestaurantDbContext(options);
			dishTypeService = new DishTypeService(dbContext);
		}

		[Test]
		public async Task AddDishTypeAsync_ShouldAddDishType()
		{
			var model = new AddDishTypeViewModel { Name = "Test Dish Type" };

			await dishTypeService.AddDishTypeAsync(model);

			var addedDishType = await dbContext.DishTypes.FirstOrDefaultAsync(dt => dt.Name == "Test Dish Type");
			Assert.NotNull(addedDishType);

			await dbContext.Database.EnsureDeletedAsync();
		}

		[Test]
		public async Task AddDishTypeAsync_ShouldThrowArgumentException_WhenDuplicateName()
		{
			var existingDishType = new DishType { Name = "Existing Type" };
			await dbContext.DishTypes.AddAsync(existingDishType);
			await dbContext.SaveChangesAsync();

			var model = new AddDishTypeViewModel { Name = "Existing Type" };

			Assert.ThrowsAsync<ArgumentException>(async () => await dishTypeService.AddDishTypeAsync(model));

			await dbContext.Database.EnsureDeletedAsync();
		}

		[Test]
		public async Task AllDishTypesAsync_ShouldReturnAllDishTypes()
		{
			var dishTypes = new List<DishType>
		{
			new DishType { Name = "Type 1" },
			new DishType { Name = "Type 2" },
			new DishType { Name = "Type 3" }
		};

			await dbContext.DishTypes.AddRangeAsync(dishTypes);
			await dbContext.SaveChangesAsync();

			var result = await dishTypeService.AllDishTypesAsync();

			Assert.That(result.Count(), Is.EqualTo(3));
			CollectionAssert.AreEquivalent(dishTypes.Select(dt => dt.Name), result.Select(dt => dt.Name));
		}

		[Test]
		public async Task DeleteDishTypeAsync_ShouldDeleteDishType()
		{
			var dishType = new DishType { Name = "To Delete" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			await dishTypeService.DeleteDishTypeAsync(dishType.Id);

			var deletedDishType = await dbContext.DishTypes.FindAsync(dishType.Id);

			Assert.True(deletedDishType!.IsDeleted == true);
		}

		[Test]
		public void DeleteDishTypeAsync_ThrowsExeptionWhenDeleteNotExistingType()
		{
			dbContext.Database.EnsureDeleted();

			int notExistingDishTypeId = 5;

			Assert.ThrowsAsync<ArgumentException>(async () => await dishTypeService.DeleteDishTypeAsync(notExistingDishTypeId));
		}

		[Test]
		public async Task EditDishTypeById_ShouldEditDishType()
		{
			var dishType = new DishType { Name = "Original Name" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var model = new AddDishTypeViewModel { Name = "New Name" };

			await dishTypeService.EditDishTypeById(model, dishType.Id);

			var editedDishType = await dbContext.DishTypes.FindAsync(dishType.Id);

			Assert.NotNull(editedDishType);
			Assert.That(editedDishType.Name, Is.EqualTo(model.Name));
		}

		[Test]
		public void EditDishTypeById_ShouldThrowExeption()
		{

			int notExistingDishTypeId = 5;
			var model = new AddDishTypeViewModel { Name = "Not Existing" };

			Assert.ThrowsAsync<ArgumentException>(async () => await dishTypeService.EditDishTypeById(model, notExistingDishTypeId));
		}

		[Test]
		public async Task GetDishTypeForEditById_ShouldReturnCorrectModel()
		{
			var dishType = new DishType { Name = "Type for Edit" };

			await dbContext.DishTypes.AddAsync(dishType);
			await dbContext.SaveChangesAsync();

			var result = await dishTypeService.GetDishTypeForEditById(dishType.Id);

			Assert.NotNull(result);
			Assert.That(result.Name, Is.EqualTo(dishType.Name));
		}
	}
}