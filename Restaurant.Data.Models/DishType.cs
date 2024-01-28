namespace Restaurant.Data.Models
{
	using System.ComponentModel.DataAnnotations;

	using static Restaurant.Common.EntityValidationConstants.DishType;

	public class DishType
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[MaxLength(DishTypeMaxLenght)]
		public string Name { get; set; } = null!;
		public bool IsDeleted { get; set; }
	}
}