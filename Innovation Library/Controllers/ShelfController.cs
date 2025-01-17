using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class ShelfController : Controller
    {
        // GET: Shelf
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewShelf()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult NewShelf()
        //{
        //    return View();
        //}
    }
}