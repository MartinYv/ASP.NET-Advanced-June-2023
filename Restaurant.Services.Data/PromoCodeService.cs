namespace Restaurant.Services.Data
{
	using Microsoft.EntityFrameworkCore;

	using Restaurant.Data;
	using Restaurant.Data.Models;
	using Restaurant.Services.Data.Interfaces;
	using Restaurant.ViewModels.Models.PromoCode;

	public class PromoCodeService : IPromoCodeService
	{
		private readonly RestaurantDbContext context;

		public PromoCodeService(RestaurantDbContext _context)
		{
			context = _context;
		}

		public async Task AddPromoCodeAsync(AddPromoCodeViewModel addPromoCode)
		{
			var promoCode = new PromoCode()
			{
				ExpirationDate = addPromoCode.ExpirationDate,
				PromoPercent = addPromoCode.PromoPercent,
				MaxUsageTimes = addPromoCode.MaxUsageTimes,
			};

			Random random = new Random();

			promoCode.Code = this.RandomString(random.Next(6, 10));

			await context.PromoCodes.AddAsync(promoCode);
			await context.SaveChangesAsync();
		}

		public async Task DeletePromoCodeAsync(int id)
		{
			PromoCode? code = await GetPromoCodeByIdAsync(id);

			if (code == null)
			{
				throw new ArgumentException("Invalid code Id.");
			}

			code.IsDeleted = true;
			await context.SaveChangesAsync();
		}

		public async Task<IEnumerable<PromoCodeViewModel>> GetAllPromoCodesAsync()
		{
			var model = await context.PromoCodes.Where(c => c.IsDeleted == false).Select(c => new PromoCodeViewModel()
			{
				Id = c.Id,
				Code = c.Code,
				ExpirationDate = c.ExpirationDate,
				MaxUsageTimes = c.MaxUsageTimes,
				PromoPercent = c.PromoPercent,
				UsedTimes = c.UsedTimes,
				IsDeleted = c.IsDeleted
			}).ToListAsync();

			return model;
		}

		public async Task<PromoCode> GetPromoCodeByIdAsync(int id)
		{
			var code = await context.PromoCodes.FindAsync(id);

			if (code == null)
			{
				throw new ArgumentException("Invalid code Id.");
			}

			return code;
		}

		public async Task<PromoCode?> GetPromoCodeByString(string codeString)
		{
			PromoCode? promoCode = await context.PromoCodes.Where(c => c.IsDeleted == false && c.Code == codeString).FirstOrDefaultAsync();

			return promoCode;
		}


		public async Task<bool> IsPromoCodeValid(int id)
		{
			PromoCode? promoCode = await context.PromoCodes.Where(c => c.IsDeleted == false).FirstOrDefaultAsync(c => c.Id == id);

			if (promoCode == null)
			{
				return false;
			}

			if (promoCode.ExpirationDate <= DateTime.UtcNow || promoCode.UsedTimes >= promoCode.MaxUsageTimes)
			{
				return false;
			}

			return true;
		}

		public async Task UsePromoCodeAsync(int id)
		{
			if (await IsPromoCodeValid(id))
			{
				var promoCode = await context.PromoCodes.FindAsync(id);

				promoCode!.UsedTimes += 1;

				if (promoCode.UsedTimes == promoCode.MaxUsageTimes)
				{
					promoCode.IsDeleted = true;
				}
			}

			await context.SaveChangesAsync();
		}

		public string RandomString(int length)
		{
			Random random = new Random();

			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var randomCode = new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());

			return randomCode;
		}
	}
}