namespace BookShop.Repository
{
	public class CheckoutViewModel
	{
		public string email { get; set; }
		public string address { get; set; }
		public string phone { get; set; }
		public int order_id { get; set; }
		public int user_id { get; set; }
		public int status { get; set; }
		public DateTime order_date { get; set; }
		public DateTime order_delivery { get; set; }
	}
}
