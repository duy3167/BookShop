using BookShop.Models;
using BookShop.Repository;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookShop.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDBContext dbContext;
        private string zone { get; set; }
        public ProfileController(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.zone = "client,owner,admin";
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

			ViewData["isLogin"] = true;

			string email = Authentication.Instance.email;
            UserViewModel user = dbContext.users
                                .Select(p => 
                                new UserViewModel {
                                     email = p.email,
                                     address = p.address,
                                     phone = p.phone,
                                     gender = p.gender,
                                }).Where(u => u.email == email).FirstOrDefault();

            return View(user);
        }

        [HttpPost]
        public IActionResult ChangesInformation(UserViewModel editUser)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            string email = Authentication.Instance.email;
            User user = dbContext.users.Where(u => u.email == email).FirstOrDefault();

            user.phone = editUser.phone;
            user.gender = editUser.gender;
            user.address = editUser.address;
            dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public class ChangePass
        {
            public string currentPass { get; set; }
            public string newPass { get; set; }
            public string confirmPass { get; set; }
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePass changePass)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            string email = Authentication.Instance.email;
            User user = dbContext.users.Where(u => u.email == email).FirstOrDefault();

            if(changePass.currentPass == user.password)
            {
               if(changePass.newPass == changePass.confirmPass)
                {
                    user.password = changePass.newPass;
                    dbContext.SaveChanges();

                    return RedirectToAction("Index");
                }
                TempData["error"] = "Confirm password not match";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Invalid current password";
            return RedirectToAction("Index");
        }
    }
}
