namespace Restaurant.Services.Data.Interfaces
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Restaurant.Data.Models;
	using Restaurant.ViewModels.Models.PromoCode;
	public interface IPromoCodeService
	{
		public Task AddPromoCodeAsync(AddPromoCodeViewModel addPromoCode);
		public Task<IEnumerable<PromoCodeViewModel>> GetAllPromoCodesAsync();
		public Task<PromoCode> GetPromoCodeByIdAsync(int id);
		public Task UsePromoCodeAsync(int id);
		public Task DeletePromoCodeAsync(int it);
		public Task<bool> IsPromoCodeValid(int id);
		public Task<PromoCode?> GetPromoCodeByString(string codeString);
		public string RandomString(int length);
	}
}