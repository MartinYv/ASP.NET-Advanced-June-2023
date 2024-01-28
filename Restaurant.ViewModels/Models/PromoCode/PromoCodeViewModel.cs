namespace Restaurant.ViewModels.Models.PromoCode
{
    public class PromoCodeViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public int MaxUsageTimes { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int UsedTimes { get; set; }
        public int PromoPercent { get; set; }
        public bool IsDeleted { get; set; }
    }
}