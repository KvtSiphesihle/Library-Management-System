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
    public class RoomsController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Rooms
        public ActionResult NewRoom()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewRoom(Room _Room)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(_Room.ImageFile.FileName);
                string extension = Path.GetExtension(_Room.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                _Room.Image = "../Content/" + fileName;
                fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
                _Room.ImageFile.SaveAs(fileName);
                _db.Rooms.Add(_Room);
                _db.SaveChanges();
            }
            return RedirectToAction("RoomIndex", "Admin");
        }
        public ActionResult Delete(int? id)
        {
            Room _Room = _db.Rooms.Find(id);
            if (_Room == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Room);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Room _Room = _db.Rooms.Find(id);
            if (_Room == null)
            {
                return HttpNotFound();
            }
            _db.Rooms.Remove(_Room);
            _db.SaveChanges();
            return RedirectToAction("RoomIndex", "Admin");
        }
    }
}