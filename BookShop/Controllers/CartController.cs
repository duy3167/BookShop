using BookShop.Models;
using BookShop.Repository;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookShop.Controllers
{
    public class CartController : Controller
    {
		private readonly AppDBContext dbContext;
		private string zone { get; set; }
		public CartController(AppDBContext dbContext)
		{
			this.dbContext = dbContext;
			this.zone = "client,owner,admin";
		}

		[HttpGet]
        public async Task<IActionResult> Index()
        {
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
			ViewData["isLogin"] = true;

			int userId = Authentication.Instance.userId;
			int orderId = this.GetOrderId(userId);
		
			ViewData["cartList"] = await dbContext.orderDetails.Include("book")
										.Join(dbContext.orders, od => od.order_id, o => o.order_id, (od, o)
										=> new OrderDetailViewModel
										{
											order_detail_id = od.order_detail_id,
											image = od.book.image,
											title = od.book.title,
											price = od.book.price,
											quantity = od.quantity,
											total = od.book.price * od.quantity,
											order_id = o.order_id,
											date = o.delivery_date,
											status = o.status,
											user_id = o.user_id,

										}).Where(od => od.status == 0 && od.user_id == userId)
										.ToListAsync();
			ViewBag.orderId = orderId;
			CheckoutViewModel checkoutInfor = await dbContext.orders.Include("user")
				.Where(o => o.order_id == orderId && o.user_id == userId && o.status == 0)
				.Select(o => new CheckoutViewModel
				{
					email = o.user.email,
					address = o.user.address,
					phone = o.user.phone,
					order_id = o.order_id,
					order_date = o.order_date,
					order_delivery = o.delivery_date,
					status = o.status,
				}).FirstOrDefaultAsync();

			return View(checkoutInfor);
        }

		private  int GetOrderId(int userId)
		{
			Order order = dbContext.orders
					.Where(o => o.user_id == userId && o.status == 0)
					.FirstOrDefault();

			if (order == null)
			{
				Order newOrder = new Order();
				newOrder.user_id = userId;
				newOrder.status = 0;
				dbContext.Add(newOrder);
				dbContext.SaveChanges();
				return newOrder.order_id;
			}
			else
			{
				return order.order_id;
			}
		}

		[HttpGet]
		public async Task<IActionResult> AddToCart(int id)
		{

			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			int userId = Authentication.Instance.userId;
			Order order = await dbContext.orders.Where(o => o.user_id == userId && o.status == 0).FirstOrDefaultAsync();

			if (order != null)
			{
				OrderDetail orderDetail = await dbContext.orderDetails
								.Where(od => od.order_id == order.order_id && od.book_id == id)
								.FirstOrDefaultAsync();
				if (orderDetail != null)
				{
					orderDetail.quantity += 1;
				}
				else
				{
					OrderDetail newOrderDetail = new OrderDetail();
					newOrderDetail.order_id = order.order_id;
					newOrderDetail.book_id = id;
					newOrderDetail.quantity = 1;
					dbContext.Add(newOrderDetail);
				}
				await dbContext.SaveChangesAsync();

			}
			else
			{
				order = new Order();
				order.user_id = userId;
				order.status = 0;
				dbContext.Add(order);
				await dbContext.SaveChangesAsync();

				OrderDetail newOrderDetail = new OrderDetail();
				newOrderDetail.order_id = order.order_id;
				newOrderDetail.book_id = id;
				newOrderDetail.quantity = 1;
				dbContext.Add(newOrderDetail);
				await dbContext.SaveChangesAsync();

			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult History()
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
			ViewData["isLogin"] = true;

			int userId = Authentication.Instance.userId;

			List<OrderDetailViewModel> history = dbContext.orderDetails.Include("book")
							.Join(dbContext.orders, od => od.order_id, o => o.order_id, (od, o)
							=> new OrderDetailViewModel{
											order_detail_id = od.order_detail_id,
											image = od.book.image,
											title = od.book.title,
											price = od.book.price,
											quantity = od.quantity,
											total = od.book.price * od.quantity,
											order_id = o.order_id,
											date = o.delivery_date,
											status = o.status,
											user_id = o.user_id,

							}).Where(od => od.status == 1 && od.user_id == userId)
							.ToList();

			return View(history);
		}

		[HttpGet]
		public IActionResult UpQuantity(int id)
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			OrderDetail orderDetail = dbContext.orderDetails.Find(id);
			if(orderDetail != null)
			{
					orderDetail.quantity++;
					dbContext.Update(orderDetail);
					dbContext.SaveChanges();
					return RedirectToAction("Index");
			}
			return NotFound();
		}

		[HttpGet]
		public IActionResult DownQuantity(int id)
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			OrderDetail orderDetail = dbContext.orderDetails.Find(id);
			if (orderDetail != null)
			{
				if (orderDetail.quantity > 1)
				{
					orderDetail.quantity--;
					dbContext.SaveChanges();
				}
			}
			return RedirectToAction("Index");
		}

		[HttpGet]
		public IActionResult Remove(int id)
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			OrderDetail orderDetail = dbContext.orderDetails.Find(id);
			dbContext.Remove(orderDetail);
			dbContext.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> CheckOut(int id)
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			int userId = Authentication.Instance.userId;
			var order = await dbContext.orders.FindAsync(id);
			
			var Payments = await dbContext.orderDetails.Include("book")
							.Join(dbContext.orders, od => od.order_id, o => o.order_id, (od, o)
							=> new PaymentViewModel
							{
								book_id = od.book.book_id,
								quantity = od.quantity,
								total = od.book.price * od.quantity,
								inventory = od.book.quantity,
								user_id = o.user_id,
								status = o.status,

							}).Where(od => od.status == 0 && od.user_id == userId)
							.ToListAsync();

			float total = 0;
			if (Payments.Count == 0)
			{
				return RedirectToAction("Index");
			}
			else
			{
				foreach (PaymentViewModel payment in Payments)
				{
					if (payment.inventory < payment.quantity)
					{
						TempData["error"] = "Out of stock";
						return RedirectToAction("Index");
					}
				}

				if (order.status == 0)
				{
					foreach (PaymentViewModel payment in Payments)
					{
						CheckInventory(payment.book_id, payment.quantity);
						total = total + payment.total;
					}

					order.order_date = DateTime.Today;
					order.delivery_date = order.order_date.AddDays(3);
					order.status = 1;
					order.total = total;
					dbContext.Update(order);
					dbContext.SaveChanges();
				}
			}
			return RedirectToAction("Index");
		}

		private void CheckInventory(int id, int quantity)
		{
			var book = dbContext.books.Find(id);
			if (book != null)
			{
				book.quantity -= quantity;
				dbContext.Update(book);
				dbContext.SaveChanges();
			}
		}

	}
}
