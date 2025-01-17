using Innovation_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class ShelvesController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Shelves
        public ActionResult Index()
        {
            return View(_db.Shelves.ToList());
        }

        public ActionResult Delete(int? id)
        {
            Shelves _Shelves = _db.Shelves.Find(id);
            if (_Shelves == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Shelves);
        }

        public ActionResult Delete(int id)
        {
            Shelves _Shelves = _db.Shelves.Find(id);
            if (_Shelves == null)
            {
                return HttpNotFound();
            }
            _db.Shelves.Remove(_Shelves);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddShelf()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddShelf(Shelves shelves)
        {
            if (ModelState.IsValid)
            {
                _db.Shelves.Add(shelves);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(shelves);
        }
    }
}