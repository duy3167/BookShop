using BookShop.Models;
using BookShop.Utils;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class SupplierController : Controller
    {
        private readonly AppDBContext dbContext;
        private string zone { get; set; }
        public SupplierController(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.zone = "owner, admin";
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            var supList = dbContext.suppliers.Where(s => s.status == 1).ToList();
            return View(supList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier sup)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            sup.status = 1;
            dbContext.Add(sup);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Supplier supplier = dbContext.suppliers.Where(c => c.sup_id == id).FirstOrDefault();

            if (supplier != null)
            {
                return View(supplier);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Supplier editSup)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Supplier supplier = dbContext.suppliers.Where(c => c.sup_id == editSup.sup_id).FirstOrDefault();
            if (supplier != null)
            {
                supplier.name = editSup.name;
                supplier.address = editSup.address;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Supplier supplier = dbContext.suppliers.Where(c => c.sup_id == id).FirstOrDefault();
            if (supplier != null)
            {
                supplier.status = 0;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

    }
}
