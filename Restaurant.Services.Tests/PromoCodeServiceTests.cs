namespace Restaurant.Services.Tests
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.PromoCode;


	[TestFixture]
	public class PromoCodeServiceTests
	{
		private DbContextOptions<RestaurantDbContext> options;
		private RestaurantDbContext context;
		private IPromoCodeService promoCodeService;

		[SetUp]
		public void Setup()
		{
			options = new DbContextOptionsBuilder<RestaurantDbContext>()
				.UseInMemoryDatabase(databaseName: "PromoCodeService_Database")
				.Options;

			context = new RestaurantDbContext(options);
			promoCodeService = new PromoCodeService(context);
		}

		[TearDown]
		public void TearDown()
		{
			context.Database.EnsureDeleted();
		}

		[Test]
		public async Task AddPromoCodeAsync_ShouldAddPromoCode()
		{
			var addPromoCodeViewModel = new AddPromoCodeViewModel
			{
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 10,
				PromoPercent = 20
			};

			await promoCodeService.AddPromoCodeAsync(addPromoCodeViewModel);
			var promoCodes = await context.PromoCodes.ToListAsync();

			Assert.That(promoCodes.Count, Is.EqualTo(1));
			Assert.That(promoCodes[0].ExpirationDate, Is.EqualTo(addPromoCodeViewModel.ExpirationDate));
			Assert.That(promoCodes[0].MaxUsageTimes, Is.EqualTo(addPromoCodeViewModel.MaxUsageTimes));
			Assert.That(promoCodes[0].PromoPercent, Is.EqualTo(addPromoCodeViewModel.PromoPercent));
			Assert.IsFalse(string.IsNullOrEmpty(promoCodes[0].Code));

			TearDown();
		}

		[Test]
		public async Task DeletePromoCodeAsync_WithValidId_ShouldDeletePromoCode()
		{
			string code = promoCodeService.RandomString(6);

			var promoCode = new PromoCode
			{
				Code = code,
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 10,
				PromoPercent = 20
			};

			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			await promoCodeService.DeletePromoCodeAsync(promoCode.Id);

			var deletedPromoCode = await context.PromoCodes.FindAsync(promoCode.Id);

			Assert.IsTrue(deletedPromoCode!.IsDeleted);

			TearDown();
		}

		[Test]
		public void DeletePromoCodeAsync_WithInvalidId_ShouldThrowException()
		{
			int invalidId = -1;

			Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await promoCodeService.DeletePromoCodeAsync(invalidId);
			});

			TearDown();
		}

		[Test]
		public async Task GetAllPromoCodesAsync_ShouldReturnAllActivePromoCodes()
		{
			var promoCode1 = new PromoCode
			{
				Code = promoCodeService.RandomString(7),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 5,
				PromoPercent = 20
			};
			var promoCode2 = new PromoCode
			{
				Code = promoCodeService.RandomString(7),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 1,
				PromoPercent = 20
			};
			var promoCode3 = new PromoCode
			{
				Code = promoCodeService.RandomString(7),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 500,
				PromoPercent = 5,
				IsDeleted = true
			};
			context.PromoCodes.AddRange(promoCode1, promoCode2, promoCode3);
			await context.SaveChangesAsync();

			var result = await promoCodeService.GetAllPromoCodesAsync();

			Assert.That(result.Count(), Is.EqualTo(2));

			TearDown();
		}

		[Test]
		public async Task GetPromoCodeByIdAsync_WithValidId_ShouldReturnPromoCode()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(7),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 5,
				PromoPercent = 20
			}; context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			var result = await promoCodeService.GetPromoCodeByIdAsync(promoCode.Id);

			Assert.That(result.Id, Is.EqualTo(promoCode.Id));

			TearDown();
		}

		[Test]
		public void GetPromoCodeByIdAsync_WithInvalidId_ShouldThrowException()
		{
			int invalidId = -1;

			Assert.ThrowsAsync<ArgumentException>(async () =>
			{
				await promoCodeService.GetPromoCodeByIdAsync(invalidId);
			});

			TearDown();
		}


		[Test]
		public async Task GetPromoCodeByString_WithInvalidCode_ShouldReturnNull()
		{
			var result = await promoCodeService.GetPromoCodeByString("InvalidCode");

			Assert.IsNull(result);
		}

		[Test]
		public async Task IsPromoCodeValid_WithValidCode_ShouldReturnTrue()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(8),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 10,
				UsedTimes = 5,
				IsDeleted = false
			};
			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			var result = await promoCodeService.IsPromoCodeValid(promoCode.Id);

			Assert.IsTrue(result);

			TearDown();
		}

		[Test]
		public async Task IsPromoCodeValid_WithExpiredCode_ShouldReturnFalse()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(8),

				ExpirationDate = DateTime.UtcNow.AddDays(-1),
				MaxUsageTimes = 10,
				UsedTimes = 5,
				IsDeleted = false
			};
			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			var result = await promoCodeService.IsPromoCodeValid(promoCode.Id);

			Assert.IsFalse(result);

			TearDown();
		}

		[Test]
		public async Task IsPromoCodeValid_WithMaxUsageReached_ShouldReturnFalse()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(8),

				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 5,
				UsedTimes = 5,
				IsDeleted = false
			};
			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			var result = await promoCodeService.IsPromoCodeValid(promoCode.Id);

			Assert.IsFalse(result);

			TearDown();
		}

		[Test]
		public async Task UsePromoCodeAsync_WithValidCode_ShouldUpdateUsedTimes()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(8),
				ExpirationDate = DateTime.UtcNow.AddDays(7),
				MaxUsageTimes = 10,
				UsedTimes = 5,
				IsDeleted = false
			};
			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			await promoCodeService.UsePromoCodeAsync(promoCode.Id);
			var updatedPromoCode = await context.PromoCodes.FindAsync(promoCode.Id);

			Assert.That(updatedPromoCode!.UsedTimes, Is.EqualTo(6));

			TearDown();
		}

		[Test]
		public async Task UsePromoCodeAsync_WithExpiredCode_ShouldNotUpdateUsedTimes()
		{
			var promoCode = new PromoCode
			{
				Code = promoCodeService.RandomString(8),

				ExpirationDate = DateTime.UtcNow.AddDays(-1),
				MaxUsageTimes = 10,
				UsedTimes = 5,
				IsDeleted = false
			};
			context.PromoCodes.Add(promoCode);
			await context.SaveChangesAsync();

			await promoCodeService.UsePromoCodeAsync(promoCode.Id);
			var updatedPromoCode = await context.PromoCodes.FindAsync(promoCode.Id);

			Assert.That(updatedPromoCode!.UsedTimes, Is.EqualTo(5));

			TearDown();
		}
	}
}