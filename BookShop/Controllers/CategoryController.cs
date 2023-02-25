using BookShop.Models;
using BookShop.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDBContext dbContext;
        private string zone { get; set; }
        public CategoryController(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.zone = "owner,admin";
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            var cateList = dbContext.categories.Where(c => c.status == 1 || c.status == 2).ToList();
            return View(cateList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();
            return View();
        }

        [HttpPost]
        public IActionResult Create(string name)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Category cate = new Category();
            cate.name = name;
            cate.status = 2;
            dbContext.Add(cate);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Category category = dbContext.categories.Where(c => c.cate_id == id).FirstOrDefault();
           
            if (category != null)
            {
                return View(category);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult Edit(Category editCate)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Category category = dbContext.categories.Where(c => c.cate_id == editCate.cate_id).FirstOrDefault();
            if(category != null)
            {
                category.name = editCate.name;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Category category = dbContext.categories.Where(c => c.cate_id == id).FirstOrDefault();
            if (category != null)
            {
                category.status = 0;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            return NotFound();
        }
    }
}
