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
			List<Category> cateList = dbContext.categories.Where(c => c.status == 2).ToList();
			return View(cateList);
        }

		[HttpPost]
		public IActionResult AcceptCate(int id)
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
            Category cate = dbContext.categories.Where(c => c.status == 2 && c.cate_id == id).FirstOrDefault();
			
			if(cate != null)
			{
				cate.status = 1;
				dbContext.SaveChanges();
				return RedirectToAction("CategoryRequest");
			}
            return NotFound();
		}

		[HttpPost]
		public IActionResult RejectCate(int id)
		{
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
            Category cate = dbContext.categories.Where(c => c.status == 2 && c.cate_id == id).FirstOrDefault();

            if (cate != null)
            {
				dbContext.Remove(cate);
                dbContext.SaveChanges();
                return RedirectToAction("CategoryRequest");
            }
            return NotFound();
        }

		[HttpGet]
		public IActionResult OwnerRequest()
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
            List<User> userList = dbContext.users.Where(u => u.status == 1 && u.role == "owner").ToList();
            return View(userList);
		}

		[HttpGet]
		public IActionResult CustomerRequest()
		{
			if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
            List<User> userList = dbContext.users.Where(u => u.status == 1 && u.role == "client").ToList();
            return View(userList);
		}

		[HttpPost]
		public IActionResult RequestResetPass(string email)
		{
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            User user = dbContext.users.Where(u => u.email == email).FirstOrDefault();
			if(user != null)
			{
				user.status = 1;
				dbContext.SaveChanges();
				ViewBag.message = $"Request is processing for email: {email}";
				return View("RequestSuccess");
			}

			ViewBag.message = $"We couldn't find an email address {email} in the system";
			return View("RequestFail");
		}

		[HttpPost]
		public IActionResult AcceptResetPass(string email)
		{
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            User user = dbContext.users.Where(u => u.email == email && u.status == 1).FirstOrDefault();

			if (user != null)
			{
				user.password = "000000";
				user.status = 0;
				dbContext.SaveChanges();
				if (user.role == "client")
				{
					return RedirectToAction("CustomerRequest");
				}

				return RedirectToAction("OwnerRequest");
			}

			return NotFound();
		}

	}
}
