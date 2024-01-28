namespace Restaurant.Services.Data
{
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;
	using System.Security.Claims;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.Order;

	public class ShoppingCartService : IShoppingCartService
	{
		private readonly RestaurantDbContext context;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IPromoCodeService promoCodeService;

		public ShoppingCartService(RestaurantDbContext _context,
								   IHttpContextAccessor _httpContextAccessor,
										   IPromoCodeService _promoCodeService)
		{
			context = _context;
			httpContextAccessor = _httpContextAccessor;
			promoCodeService = _promoCodeService;
		}
		public async Task AddItem(int dishId, int qty)
		{
			string? userId = GetUserId();

			if (string.IsNullOrEmpty(userId))
			{
				throw new ArgumentException("user is not logged-in");

			}

			var cart = await GetCart(userId);

			if (cart == null)
			{
				cart = new ShoppingCart
				{
					UserId = Guid.Parse(userId)
				};

				context.ShoppingCarts.Add(cart);
			}
			await context.SaveChangesAsync();


			var cartItem = await context.CartDetails
							  .FirstOrDefaultAsync(a => a.ShoppingCartId == cart.Id && a.DishId == dishId);

			if (cartItem != null)
			{
				cartItem.Quantity += qty;
			}
			else
			{
				var dish = await context.Dishes.FindAsync(dishId);

				if (dish == null)
				{
					throw new ArgumentException("Invalid dish");
				}
				cartItem = new CartDetail
				{
					Dish = dish,
					DishId = dishId,
					ShoppingCartId = cart.Id,
					Quantity = qty,
					UnitPrice = dish.Price
				};

				await context.CartDetails.AddAsync(cartItem);
			}

			await context.SaveChangesAsync();
		}

		public async Task RemoveItem(int dishId)
		{
			string? userId = GetUserId();

			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("user is not logged-in");

			}

			var cart = await GetCart(userId);

			if (cart == null)
			{
				throw new Exception("Invalid cart");
			}

			var cartItem = context.CartDetails.FirstOrDefault(a => a.ShoppingCartId == cart.Id && a.DishId == dishId);

			if (cartItem == null)
			{
				throw new Exception("Not items in cart");

			}
			else if (cartItem.Quantity == 1)
			{
				context.CartDetails.Remove(cartItem);

			}
			else
			{
				cartItem.Quantity = cartItem.Quantity - 1;
			}

			await context.SaveChangesAsync();
		}

		public async Task<ShoppingCart?> GetUserCart()
		{
			if (string.IsNullOrWhiteSpace(GetUserId()))
			{
				throw new ArgumentException("First you have to login.");
			}

			Guid? userId = Guid.Parse(GetUserId()!);

			var shoppingCart = await context.ShoppingCarts
								  .Include(a => a.CartDetails)
								  .ThenInclude(a => a.Dish)
								  .ThenInclude(a => a.DishType)
								  .Where(a => a.UserId == userId).FirstOrDefaultAsync();

			return shoppingCart;
		}
		public async Task<ShoppingCart?> GetCart(string userId)
		{
			var cart = await context.ShoppingCarts.FirstOrDefaultAsync(x => x.UserId == Guid.Parse(userId));
			return cart;
		}

		public async Task<bool> DoCheckout(OrderUsersInfoViewModel usersInfo)
		{
			var userId = GetUserId();

			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("User is not logged-in");
			}

			var user = await context.Users.FindAsync(Guid.Parse(userId));

			if (user == null)
			{
				throw new ArgumentException("Invalid user.");
			}

			var cart = await GetCart(userId);

			if (cart == null)
			{
				throw new Exception("Invalid cart");
			}

			var cartDetail = await context.CartDetails
								.Where(a => a.ShoppingCartId == cart.Id).Include(a => a.Dish).ToListAsync();

			if (cartDetail.Count == 0)
			{
				throw new Exception("Cart is empty");
			}

			bool applyPromoCode = false;
			PromoCode? promoCode = null;

			if (!string.IsNullOrWhiteSpace(usersInfo.PromoCode))
			{
				 promoCode = await promoCodeService.GetPromoCodeByString(usersInfo.PromoCode);

				if (promoCode != null)
				{
					bool IsPromoCodeValid = await promoCodeService.IsPromoCodeValid(promoCode.Id);

					if (IsPromoCodeValid)
					{
						await promoCodeService.UsePromoCodeAsync(promoCode.Id);
						applyPromoCode = true;
					}
				}
			}

			var order = new Order
			{
				FirstName = usersInfo.FirstName,
				LastName = usersInfo.LastName,
				Address = usersInfo.Address,
				Phone = usersInfo.Phone,
				CustomerId = Guid.Parse(userId),
				CreateDate = DateTime.UtcNow,
				IsCompleted = false,
				IsDeleted = false,
				PromoCode = promoCode,
			};

			await context.Orders.AddAsync(order);
			await context.SaveChangesAsync();

			foreach (var item in cartDetail)
			{

				var orderDetail = new OrderDetail
				{
					Dish = item.Dish,
					DishId = item.DishId,
					OrderId = order.Id,
					Quantity = item.Quantity,
					UnitPrice = item.UnitPrice
				};

				await context.OrderDetails.AddAsync(orderDetail);
			}

			order.Price = order.OrderDetail.Sum(x => x.Quantity * x.UnitPrice);

			if (applyPromoCode)
			{
				order.Price -= order.Price * ((decimal)promoCode!.PromoPercent / 100);
			}

			await context.SaveChangesAsync();

			context.CartDetails.RemoveRange(cartDetail);

			user.OrdersPlaced.Add(order);

			await context.SaveChangesAsync();

			return true;
		}

		private string? GetUserId()
		{
			string? userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

			return userId;
		}
	}
}