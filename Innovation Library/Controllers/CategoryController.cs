using Innovation_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class CategoryController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Category
        public ActionResult Index()
        {
            return View(_db.Categories.ToList());
        }

        public ActionResult NewCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult NewCategory(Category _Category)
        {
            if (ModelState.IsValid)
            {
                _db.Categories.Add(_Category);
                _db.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            ViewBag.Error = "Required Field";
            return View();
        }
    }
}