using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class RequestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
