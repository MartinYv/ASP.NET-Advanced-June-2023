namespace Restaurant.Services.Data
{
	using System.Security.Claims;
	using Microsoft.AspNetCore.Http;
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.Services.Data.Models.Order;
	using Restaurant.ViewModels.Models.Order;
	using Restaurant.ViewModels.Order.Enum;

	public class OrderService : IOrderService
	{
		private readonly RestaurantDbContext context;
		private readonly IHttpContextAccessor httpContextAccessor;

		public OrderService(RestaurantDbContext _context,
							IHttpContextAccessor _httpContextAccessor)
		{
			context = _context;
			httpContextAccessor = _httpContextAccessor;
		}

		public async Task<IEnumerable<OrderViewModel>> AllOrdersAcync()
		{
			var model = await context.Orders.Where(o => o.IsDeleted == false).Include(o => o.PromoCode).Include(o => o.OrderDetail).ThenInclude(o => o.Dish).Select(o => new OrderViewModel()
			{
				FirstName = o.FirstName,
				LastName = o.LastName,
				Address = o.Address,
				Phone = o.Phone,
				Price = o.Price.ToString(),
				CustomerId = o.CustomerId,
				CreateDate = o.CreateDate.ToString("MM/dd/yy H:mm:ss"),
				IsCompleted = o.IsCompleted ? "Delivered" : "Pending",
				OrderDetail = o.OrderDetail,
				PromoCode = o.PromoCode == null ? "None" : o.PromoCode.Code
			}).ToListAsync();

			return model;
		}

		public string? GetUserId()
		{
			string? userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
			return userId;
		}

		public async Task<AllOrdersFilteredServiceModel> UserOrdersAsync(AllOrdersQueryViewModel queryModel)
		{
			string? userId = GetUserId();

			if (userId == null)
			{
				throw new ArgumentException("Invalid user id.");
			}

			ApplicationUser? user = context.Users.Include(u => u.OrdersPlaced).ThenInclude(o => o.OrderDetail).FirstOrDefault(x => x.Id == Guid.Parse(userId));

			if (user == null)
			{
				throw new ArgumentException("Invalid user.");
			}

			var ordersQuery = context.Orders.Where(o => o.CustomerId == Guid.Parse(userId)).Include(o => o.OrderDetail).ThenInclude(o => o.Dish).AsQueryable();

			ordersQuery = queryModel.OrderSorting switch
			{
				OrderSorting.Newest => ordersQuery
					.OrderByDescending(h => h.CreateDate),
				OrderSorting.Oldest => ordersQuery
					.OrderBy(h => h.CreateDate),
				OrderSorting.PriceAscending => ordersQuery
					.OrderBy(h => h.Price),
				OrderSorting.PriceDescending => ordersQuery
					.OrderByDescending(h => h.Price),
				OrderSorting.Pending => ordersQuery
				.Where(o => o.IsCompleted == false),
				OrderSorting.Delivered => ordersQuery
				.Where(o => o.IsCompleted == true),
				_ => ordersQuery
					.OrderByDescending(h => h.CreateDate)
			};

			IEnumerable<OrderViewModel> allOrders = await ordersQuery
					   .Where(o => o.IsDeleted == false)
					   .Skip((queryModel.CurrentPage - 1) * queryModel.OrdersPerPage)
					   .Take(queryModel.OrdersPerPage)
					   .Select(o => new OrderViewModel
					   {
						   //Id = h.Id.ToString(),
						   FirstName = o.FirstName,
						   LastName = o.LastName,
						   Address = o.Address,
						   CreateDate = o.CreateDate.ToString("MM/dd/yy H:mm:ss"),
						   IsCompleted = o.IsCompleted ? "Delivered" : "Pending",
						   Price = o.Price.ToString(),
						   Phone = o.Phone,
						   OrderDetail = o.OrderDetail,
						   PromoCode = o.PromoCode == null ? "None" : o.PromoCode.Code
					   })
					   .ToListAsync();

			int totalOrders = ordersQuery.Count();

			return new AllOrdersFilteredServiceModel()
			{
				TotalOrdersCount = totalOrders,
				Orders = allOrders,
			};
		}

		public async Task<AllOrdersFilteredServiceModel> AllFilteredAsync(AllOrdersQueryViewModel queryModel)
		{
			var ordersQuery = context.Orders.Include(o => o.PromoCode).Include(o => o.OrderDetail).ThenInclude(o => o.Dish).AsQueryable();

			ordersQuery = queryModel.OrderSorting switch
			{
				OrderSorting.Newest => ordersQuery
					.OrderByDescending(h => h.CreateDate),
				OrderSorting.Oldest => ordersQuery
					.OrderBy(h => h.CreateDate),
				OrderSorting.PriceAscending => ordersQuery
					.OrderBy(h => h.Price),
				OrderSorting.PriceDescending => ordersQuery
					.OrderByDescending(h => h.Price),
				OrderSorting.Pending => ordersQuery
				.Where(o => o.IsCompleted == false),
				OrderSorting.Delivered => ordersQuery
				.Where(o => o.IsCompleted == true),
				_ => ordersQuery
					.OrderByDescending(h => h.CreateDate)
			};

			IEnumerable<OrderViewModel> allOrders = await ordersQuery
		   .Where(o => o.IsDeleted == false)
		   .Skip((queryModel.CurrentPage - 1) * queryModel.OrdersPerPage)
		   .Take(queryModel.OrdersPerPage)
		   .Select(o => new OrderViewModel
		   {
			   Id = o.Id,
			   FirstName = o.FirstName,
			   LastName = o.LastName,
			   Address = o.Address,
			   CreateDate = o.CreateDate.ToString("MM/dd/yy H:mm:ss"),
			   IsCompleted = o.IsCompleted ? "Delivered" : "Pending",
			   Price = o.Price.ToString(),
			   Phone = o.Phone,
			   OrderDetail = o.OrderDetail,
			   PromoCode = o.PromoCode == null ? "None" : o.PromoCode.Code
		   }).ToListAsync();

			int totalOrders = ordersQuery.Count();

			return new AllOrdersFilteredServiceModel()
			{
				TotalOrdersCount = totalOrders,
				Orders = allOrders
			};
		}

		public async Task<Order?> FindOrderByIdAsync(int orderId)
		{
			return await context.Orders.FindAsync(orderId);
		}

		public async Task ChangeStatusByIdAsync(int orderId)
		{
			Order? order = await FindOrderByIdAsync(orderId);

			if (order == null)
			{
				throw new ArgumentException("Invalid order id");
			}

			if (order.IsCompleted == false)
			{
				order.IsCompleted = true;
			}
			else
			{
				order.IsCompleted = false;
			}

			await context.SaveChangesAsync();
		}
	}
}