using BookShop.Models;
using BookShop.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BookShop.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDBContext dbContext;
        private string zone { get; set; }
        public BookController(AppDBContext dbContext)
        {
            this.dbContext = dbContext;
            this.zone = "owner, admin";
        }

        public IActionResult Index()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            var book = dbContext.books.Where(b => b.status == 1).Include("category").Include("supplier").ToList();
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            var cateList = await dbContext.categories.Where(c => c.status == 1).ToListAsync();
            var supList = await dbContext.suppliers.Where(s => s.status == 1).ToListAsync();
            ViewData["cateList"] = cateList;
            ViewData["supList"] = supList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] Book book, IFormFile image)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            if (ModelState.IsValid)
            {
                string folder = "img/book";
                string fileName = await FileHandler.Instance.SaveFileAsync(image, folder);
                book.image = fileName;
                book.status = 1;

                dbContext.Add(book);
                dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            var cateList = await dbContext.categories.Where(c => c.status == 1).ToListAsync();
            var supList = await dbContext.suppliers.Where(s => s.status == 1).ToListAsync();
            ViewData["cateList"] = cateList;
            ViewData["supList"] = supList;
            TempData["error"] = "Image is required";
            return View(book);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Book book = await dbContext.books.Where(b => b.book_id == id).FirstOrDefaultAsync();
            if(book != null)
            {
                var cateList = await dbContext.categories.Where(c => c.status == 1).ToListAsync();
                var supList = await dbContext.suppliers.Where(s => s.status == 1).ToListAsync();
                ViewData["cateList"] = cateList;
                ViewData["supList"] = supList;
                return View(book);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromForm] Book book, IFormFile? image)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            if (ModelState.IsValid)
            {
                Book updateBook = await dbContext.books
                                           .Where(b => b.status == 1 && b.book_id == book.book_id)
                                           .FirstOrDefaultAsync();
                if (updateBook != null)
                {
                    updateBook.title = book.title;
                    updateBook.author = book.author;
                    updateBook.description = book.description;
                    updateBook.price = book.price;
                    updateBook.cate_id = book.cate_id;
                    updateBook.sup_id = book.sup_id;
                    updateBook.quantity = book.quantity;

                    if (image != null)
                    {
                        string oldImg = updateBook.image;
                        string folder = "img/book";
                        FileHandler.Instance.DeleteFileAsync(oldImg, folder);
                        string fileName = await FileHandler.Instance.SaveFileAsync(image, folder);
                            
                        if (fileName != "")
                        {
                                updateBook.image = fileName;
                        }
                    }

                    dbContext.Update(updateBook);
                    dbContext.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            else
            {
                ViewData["error"] = "Invalid information";
            }

            var cateList = await dbContext.categories.Where(c => c.status == 1).ToListAsync();
            var supList = await dbContext.suppliers.Where(s => s.status == 1).ToListAsync();
            ViewData["cateList"] = cateList;
            ViewData["supList"] = supList;
            return View(book);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!Authentication.Instance.Authorization(HttpContext, this.zone)) return Unauthorized();

            Book deleteBook = await dbContext.books
                           .Where(b => b.status == 1 && b.book_id == id)
                           .FirstOrDefaultAsync();

            if (deleteBook != null)
            {
                string fileName = deleteBook.image;
                string folder = "img/book";
                FileHandler.Instance.DeleteFileAsync(fileName, folder);

                deleteBook.status = 0;
                dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }

        }

    }
}
