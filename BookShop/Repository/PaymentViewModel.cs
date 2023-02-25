namespace BookShop.Repository
{
	public class PaymentViewModel
	{
		public int quantity { get; set; }

		public int book_id { get; set; }

		public float total { get; set; }

		public int inventory { get; set; }

		public int status { get; set; }

		public int user_id { get; set; }
	}
}
