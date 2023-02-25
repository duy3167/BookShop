using BookShop.Models;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookShop.Controllers
{
    public class OrderController : Controller
    {
		private readonly AppDBContext dbContext;
		private string zone { get; set; }
		public OrderController(AppDBContext dbContext)
		{
			this.dbContext = dbContext;
			this.zone = "owner,admin";
		}
		public IActionResult Index()
        {
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			var orderList = dbContext.orders.Include("user").ToList();
			return View(orderList);
        }

        public IActionResult Detail(int id)
        {
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			var orderDetailList = dbContext.orderDetails.Include("order").Include("book").Where(od => od.order.order_id == id).ToList();
			return View(orderDetailList);
        }



	}
}
