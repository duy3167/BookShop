using BookShop.Models;
using BookShop.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class RequestController : Controller
    {
		private readonly AppDBContext dbContext;
		private string zone { get; set; }
		public RequestController(AppDBContext dbContext)
		{
			this.dbContext = dbContext;
			this.zone = "admin";
		}

		[HttpGet]
        public IActionResult CategoryRequest()
        {
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
			return View();
        }

		[HttpGet]
		public IActionResult OwnerRequest()
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
			return View();
		}

		[HttpGet]
		public IActionResult CustomerRequest()
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
			return View();
		}

		[HttpPost]
		public IActionResult RequestResetPass(string email)
		{
			User user = dbContext.users.Where(u => u.email == email).FirstOrDefault();
			if(user != null)
			{
				user.status = 1;
				ViewBag.message = $"Request is processing for email: {email}";
				return View("RequestSuccess");
			}

			ViewBag.message = $"We couldn't find an email address {email} in the system";
			return View("RequestFail");
		}

	}
}
