namespace Restaurant.Data.Models
{
	using Microsoft.AspNetCore.Identity;
	public class ApplicationUser : IdentityUser<Guid>
	{
		public ApplicationUser()
		{
			Id = Guid.NewGuid();
		}
		public List<Order> OrdersPlaced { get; set; } = new List<Order>();
	}
}
