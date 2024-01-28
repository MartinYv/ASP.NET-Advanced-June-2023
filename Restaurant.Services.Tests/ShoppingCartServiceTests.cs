namespace Restaurant.Services.Tests
{
	using System;
	using System.Linq;
	using System.Security.Claims;
	using System.Threading.Tasks;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.AspNetCore.Http;
	using Moq;
	using NUnit.Framework;

	using Restaurant.Services.Data;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.ViewModels.Models.Order;


	[TestFixture]
	public class ShoppingCartServiceTests
	{
		private IPromoCodeService promoCodeService;
		private IShoppingCartService shoppingCartService = null!;
		private IOrderService orderService = null!;

		private RestaurantDbContext dbContext;
		private IHttpContextAccessor httpContextAccessor;

		[SetUp]
		public void Setup()
		{
			var options = new DbContextOptionsBuilder<RestaurantDbContext>()
				.UseInMemoryDatabase(databaseName: "TestDatabase")
				.Options;

			dbContext = new RestaurantDbContext(options);
			httpContextAccessor = new HttpContextAccessor();

			orderService = new OrderService(dbContext, httpContextAccessor);
			promoCodeService = new PromoCodeService(dbContext);
		}

		[TearDown]
		public void TearDown()
		{
			dbContext.Database.EnsureDeleted();
		}

		[Test]
		public void AddItem_ShouldThrowException_WhenUserIsNotLoggedIn()
		{
			var dishId = 1;
			var quantity = 2;

			MockHttpContextAccessor(string.Empty);

			Assert.ThrowsAsync<ArgumentException>(async () => await shoppingCartService.AddItem(dishId, quantity));
		}

		[Test]
		public async Task AddItem_ShouldCreateCartAndCreateDetail()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			MockHttpContextAccessor(user.Id.ToString());

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10.33m

			};

			await dbContext.Dishes.AddAsync(dish);
			await dbContext.SaveChangesAsync();
			
			var quantity = 2;

			await shoppingCartService.AddItem(dish.Id, quantity);

			var cart = await dbContext.ShoppingCarts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.UserId == Guid.Parse(user.Id.ToString()));

			Assert.NotNull(cart);
			Assert.That(dbContext.ShoppingCarts.Count(), Is.EqualTo(1));

			Assert.That(cart.CartDetails.Count, Is.EqualTo(1));
			Assert.That(cart.CartDetails.First().DishId, Is.EqualTo(dish.Id));
		}

		[Test]
		public async Task AddItem_ShouldIncreaseQuantity_WhenCartItemExists()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			MockHttpContextAccessor(user.Id.ToString());

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10m

			};

			await dbContext.Dishes.AddAsync(dish);
			await dbContext.SaveChangesAsync();

			var quantity = 2;

			dbContext.ShoppingCarts.Add(new ShoppingCart { UserId = Guid.Parse(user.Id.ToString()) });
			dbContext.CartDetails.Add(new CartDetail { ShoppingCartId = 1, DishId = dish.Id, Quantity = 3, UnitPrice = 10 });

			await dbContext.SaveChangesAsync();

			await shoppingCartService.AddItem(dish.Id, quantity);

			var cartDetail = await dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.DishId == dish.Id);

			Assert.That(cartDetail, Is.Not.Null);
			Assert.That(cartDetail.Quantity, Is.EqualTo(5));
		}

		[Test]
		public void RemoveItem_ShouldThrowException_WhenUserIsNotLoggedIn()
		{
			var dishId = 1;

			MockHttpContextAccessor(string.Empty);

			Assert.ThrowsAsync<Exception>(async () => await shoppingCartService.RemoveItem(dishId));
		}

		[Test]
		public async Task RemoveItem_ShouldThrowException_WhenCartIsNull()
		{
			var dishId = 1;

			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

			Assert.ThrowsAsync<Exception>(async () => await shoppingCartService.RemoveItem(dishId));
		}

		[Test]
		public async Task RemoveItem_ShouldThrowException_WhenCartItemIsNull()
		{
			int dishId = 1;

			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

			dbContext.ShoppingCarts.Add(new ShoppingCart { UserId = Guid.Parse(user.Id.ToString()) });

			await dbContext.SaveChangesAsync();

			Assert.ThrowsAsync<Exception>(async () => await shoppingCartService.RemoveItem(dishId));
		}

		[Test]
		public async Task RemoveItem_ShouldRemoveCartItem_WhenQuantityIsOne()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);
			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10

			};

			dbContext.Dishes.Add(dish);
			dbContext.ShoppingCarts.Add(new ShoppingCart { UserId = Guid.Parse(user.Id.ToString()) });
			dbContext.CartDetails.Add(new CartDetail { ShoppingCartId = 1, DishId = dish.Id, Quantity = 1, UnitPrice = 10 });

			await dbContext.SaveChangesAsync();

			await shoppingCartService.RemoveItem(dish.Id);

			var cart = await dbContext.ShoppingCarts.Include(c => c.CartDetails).FirstOrDefaultAsync(c => c.UserId == Guid.Parse(user.Id.ToString()));

			Assert.NotNull(cart);
			Assert.That(cart.CartDetails.Count, Is.EqualTo(0));
		}

		[Test]
		public async Task RemoveItem_ShouldDecreaseQuantity_WhenCartItemExists()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);
			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10
			};

			dbContext.Dishes.Add(dish);
			dbContext.ShoppingCarts.Add(new ShoppingCart { UserId = Guid.Parse(user.Id.ToString()) });
			dbContext.CartDetails.Add(new CartDetail { ShoppingCartId = 1, DishId = dish.Id, Quantity = 3, UnitPrice = 10 });

			await dbContext.SaveChangesAsync();

			await shoppingCartService.RemoveItem(dish.Id);

			var cartDetail = await dbContext.CartDetails.FirstOrDefaultAsync(cd => cd.DishId == dish.Id);

			Assert.NotNull(cartDetail);
			Assert.That(cartDetail.Quantity, Is.EqualTo(2));
		}

		[Test]
		public void GetUserCart_ShouldThrowException_WhenUserIsNotLoggedIn()
		{
			MockHttpContextAccessor(string.Empty);

			Assert.ThrowsAsync<ArgumentException>(async () => await shoppingCartService.GetUserCart());
		}

		[Test]
		public async Task GetUserCart_ShouldReturnCart_WhenUserIsLoggedIn()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

			dbContext.ShoppingCarts.Add(new ShoppingCart { UserId = user.Id });

			await dbContext.SaveChangesAsync();

			var cart = await shoppingCartService.GetUserCart();

			Assert.That(cart, Is.Not.Null);
			Assert.That(cart.UserId, Is.EqualTo(Guid.Parse(user.Id.ToString())));
		}
		
		[Test]
		public async Task GetUserCart_ShouldReturnNull()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();

			MockHttpContextAccessor(user.Id.ToString());

	
			var cart = await shoppingCartService.GetUserCart();

			Assert.That(cart, Is.Null);
		}

		[Test]
		public void DoCheckout_ShouldThrowException_WhenUserIsNotLoggedIn()
		{
			MockHttpContextAccessor(string.Empty);

			Assert.ThrowsAsync<Exception>(async () => await shoppingCartService.DoCheckout(new OrderUsersInfoViewModel()));
		}

		[Test]
		public void DoCheckout_ShouldThrowException_WhenInvalidUser()
		{
			MockHttpContextAccessor(new Guid().ToString());

			Assert.ThrowsAsync<ArgumentException>(async () => await shoppingCartService.DoCheckout(new OrderUsersInfoViewModel()));
		}

		[Test]
		public async Task DoCheckout_ShouldThrowException_WhenCartIsEmpty()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();
			MockHttpContextAccessor(user.Id.ToString());

			Assert.ThrowsAsync<Exception>(async () => await shoppingCartService.DoCheckout(new OrderUsersInfoViewModel()));
		}
		
		[Test]
		public async Task DoCheckout_ShouldApplyDiscount_WhenValidPromoCodeIsProvided()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();
			MockHttpContextAccessor(user.Id.ToString());

			PromoCode code = new PromoCode()
			{
				Code = promoCodeService.RandomString(8),
				ExpirationDate = DateTime.UtcNow.AddDays(5),
				MaxUsageTimes = 5,
				UsedTimes = 1,
				PromoPercent = 10
			};

			await dbContext.AddAsync(code);

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10
			};

			await dbContext.Dishes.AddAsync(dish);
			
			ShoppingCart cart = new ShoppingCart()
			{
				User = user,
				UserId = user.Id
			};

			await dbContext.ShoppingCarts.AddAsync(cart);

			CartDetail cartDetail = new CartDetail()
			{
				Dish = dish,
				DishId = dish.Id,
				Quantity = 1,
				UnitPrice = 10,
				ShoppingCart = cart,
				ShoppingCartId = cart.Id
			};
			await dbContext.CartDetails.AddAsync(cartDetail);

			cart.CartDetails.Add(cartDetail);
			await dbContext.SaveChangesAsync();

			OrderUsersInfoViewModel usersInfo = new OrderUsersInfoViewModel()
			{
				FirstName = "John",
				LastName = "Wick",
				Address = "Test Address",
				Phone = "Test Phone",
				PromoCode = code.Code		
			};

			await shoppingCartService.DoCheckout(usersInfo);

			var order = await orderService.FindOrderByIdAsync(1);
			Assert.That(order!.Price, Is.EqualTo(9));
			Assert.That(order.PromoCode, Is.Not.Null);
		}
		
		
		[Test]
		public async Task DoCheckout_ShouldNotApplyDiscount_WhenInvalidPromoCodeIsProvided()
		{
			ApplicationUser user = new ApplicationUser() { Id = new Guid() };
			dbContext.Users.Add(user);

			await dbContext.SaveChangesAsync();
			MockHttpContextAccessor(user.Id.ToString());

			var dish = new Dish
			{
				Id = 1,
				Name = "Test Dish",
				Description = "Test Description",
				DishTypeId = 1,
				ImageUrl = "test.jpg",
				Price = 10
			};

			await dbContext.Dishes.AddAsync(dish);
			
			ShoppingCart cart = new ShoppingCart()
			{
				User = user,
				UserId = user.Id
			};

			await dbContext.ShoppingCarts.AddAsync(cart);

			CartDetail cartDetail = new CartDetail()
			{
				Dish = dish,
				DishId = dish.Id,
				Quantity = 1,
				UnitPrice = 10,
				ShoppingCart = cart,
				ShoppingCartId = cart.Id
			};
			await dbContext.CartDetails.AddAsync(cartDetail);

			cart.CartDetails.Add(cartDetail);
			await dbContext.SaveChangesAsync();

			OrderUsersInfoViewModel usersInfo = new OrderUsersInfoViewModel()
			{
				FirstName = "John",
				LastName = "Wick",
				Address = "Test Address",
				Phone = "Test Phone",
				PromoCode = "Invalid"		
			};

			await shoppingCartService.DoCheckout(usersInfo);

			var order = await orderService.FindOrderByIdAsync(1);

			Assert.That(order!.Price, Is.EqualTo(10));
		}

		/// <summary>
		/// This method mocks the HttpContextAccessor and replace it with new. 
		/// If its not invoked, the HttpContextAccessor is awlays NULL in ShoppingCartService!
		/// </summary>
		/// <param name="userId"></param>
		private void MockHttpContextAccessor(string userId)
		{
			var claims = new Claim[] { new Claim(ClaimTypes.NameIdentifier, userId) };
			var identity = new ClaimsIdentity(claims);
			var claimsPrincipal = new ClaimsPrincipal(identity);

			var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
			mockHttpContextAccessor.Setup(a => a.HttpContext.User).Returns(claimsPrincipal);

			httpContextAccessor = mockHttpContextAccessor.Object;

			shoppingCartService = new ShoppingCartService(dbContext, httpContextAccessor, promoCodeService);
		}
	}
}
