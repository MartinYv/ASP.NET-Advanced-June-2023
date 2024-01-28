namespace Restaurant.Services.Data.Interfaces
{
    using Restaurant.Data.Models;
    using Restaurant.ViewModels.Models.Order;
    public interface IShoppingCartService
    {
        Task AddItem(int dishId, int qty);
        Task RemoveItem(int dishId);
        Task<ShoppingCart?> GetUserCart();
        Task<ShoppingCart?> GetCart(string userId);
        Task<bool> DoCheckout(OrderUsersInfoViewModel usersInfo);
    }
}