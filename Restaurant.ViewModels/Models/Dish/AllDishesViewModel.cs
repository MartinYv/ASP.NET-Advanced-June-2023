namespace Restaurant.ViewModels.Models.Dish
{
    public class AllDishesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ImageUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public string DishType { get; set; } = null!;
    }
}
