using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BookShop.Utils;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext dbContext;
        public HomeController(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Index()
        {
            this.HasSession();
            ICollection<Book> bookList = dbContext.books
                               .Where(b => b.status == 1)
                               .Include(c => c.category)
                               .Include(s => s.supplier).ToList();
            return View(bookList);
        }

        public IActionResult Book()
        {
            this.HasSession();
            ICollection<Book> bookList  = dbContext.books
                                           .Where(b => b.status == 1)
                                           .Include(c => c.category)
                                           .Include(s => s.supplier).ToList();
         
            return View(bookList);
        }

        public IActionResult Detail(int id)
        {
            this.HasSession();
            Book book = dbContext.books.Where(b => b.book_id == id).FirstOrDefault();
            if(book != null)
            {
                return View(book);
            }
            return NotFound();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] string email, [FromForm] string password)
        {
            User user = dbContext.users.Where(u => u.email == email && u.password == password).FirstOrDefault();
            if(user != null)
            {
                Authentication.Instance.SetSession(user.user_id, user.email, user.role, HttpContext);
                if (user.role == "admin")
                {
                    return RedirectToAction("CategoryRequest", "Request");
                }
                else if (user.role == "owner")
                {
                    return RedirectToAction("Index", "Book");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            TempData["error"] = "Email or password wrong";
            return View();

        }

        private void HasSession()
        {
            Authentication.Instance.BindingSession(HttpContext);
            if (Authentication.Instance.userId != 0)
            {
                ViewData["isLogin"] = true;
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

		[HttpPost]
		public IActionResult Register([FromForm] User user)
		{
            if (ModelState.IsValid)
            {
				var checkEmail = dbContext.users.Where(u => u.email == user.email).FirstOrDefault();
				if (checkEmail != null)
				{
					TempData["error"] = "Email already used";
				}
				else
				{
                    user.role = "client";
                    user.status = 0;
                    dbContext.Add(user);
                    dbContext.SaveChanges();
                    return RedirectToAction("Login");
				}
            }

			return View();
		}

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult NotFound()
        {
            return View();
        }
    }
}