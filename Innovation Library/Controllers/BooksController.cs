using Innovation_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class BooksController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Books
        public ActionResult Index()
        {
            return View(_db.Books.ToList());
        }

        public ActionResult Restock(int? id)
        {
            return View();
        }
        [HttpPost]
        public ActionResult Restock(int id, Book book)
        {
            if (id.Equals(0))
            {
                return HttpNotFound();
            }
            Book _book = _db.Books.Find(id);
            _book.Quantity = book.Quantity;
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
        {
            Book _Book = _db.Books.Find(id);
            if (_Book == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Book);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Book _Book = _db.Books.Find(id);
            if (_Book == null)
            {
                return HttpNotFound();
            }
            _db.Books.Remove(_Book);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult NewBook()
        {
            var Shelf = _db.Shelves.ToList();
            var Categories = _db.Categories.ToList();
            ViewBag.ShelfNo = new SelectList(Shelf, "ShelfId", "ShelfNo");
            ViewBag.Categories = new SelectList(Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        public ActionResult NewBook(Book _Book)
        {
            Category _Category = _db.Categories.Find(Convert.ToInt32(_Book.Category));
            if (_Category != null)
            {
                _Book.Category = _Category.CategoryName;
            }
            DateTime Date = Convert.ToDateTime(_Book.PublishedDate);
            _Book.PublishedDate = Date.ToShortDateString().ToString();
            string fileName = Path.GetFileNameWithoutExtension(_Book.ImageFile.FileName);
            string extension = Path.GetExtension(_Book.ImageFile.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            _Book.Image = "../Content/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
            _Book.ImageFile.SaveAs(fileName);
            _db.Books.Add(_Book);
            _db.SaveChanges();
             
            return RedirectToAction("Index", "Books");
        }

        public ActionResult BookDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Book _Book = _db.Books.Find(id);
            if (_Book == null)
            {
                return HttpNotFound();
            }

            return View(_Book);
        }  
    }
}