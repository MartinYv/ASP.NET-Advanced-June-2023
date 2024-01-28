namespace Restaurant.Common
{
	public static class EntityValidationConstants
	{
		public static class Dish
		{
			public const int DishNameMinLength = 4;
			public const int DishNameMaxLength = 35;

			public const int DishDescriptionMinLength = 10;
			public const int DishDescriptionMaxLength = 70;

			public const int DishPriceMinLength = 5;
			public const int DishPriceMaxLength = 50;

			public const int DishUrlMinLength = 30;
			public const int DishUrlMaxLength = 2048;
		}

		public static class DishType
		{
			public const int DishTypeMinLength = 4;
			public const int DishTypeMaxLenght = 15;
		}

		public static class Order
		{
			public const int FirstNameMinLength = 2;
			public const int FirstNameMaxLength = 30;
			
			public const int LastNameMinLength = 2;
			public const int LastNameMaxLength = 30;

			public const int PhoneMinLength = 7;
			public const int PhoneMaxLength = 20;

			public const int AddressMinLength = 6;
			public const int AddressMaxLength = 70;
		}

		public static class Menu
		{
			public const int MenuUrlMinLength = 30;
			public const int MenuUrlMaxLength = 2048;
		}

		public static class PromoCode
		{
			public const int CodeMinLength = 4;
			public const int CodeMaxLength = 10;
		}

	}
}