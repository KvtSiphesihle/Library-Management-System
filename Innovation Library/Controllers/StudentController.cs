using Innovation_Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class StudentController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Student
        [Authorize]
        public ActionResult Dashboard()
        {
            var ActiveStudentId = User.Identity.GetUserId();
            var ActiveStudentData = _db.Students.Where(s => s.StudentGuid == ActiveStudentId).FirstOrDefault();
            var RecentBorrows = _db.Borrowers.Where(rs=>rs.StudentId == ActiveStudentId).ToList();

            ViewBag.Borrowers = RecentBorrows;

            return View(ActiveStudentData);
        }

        [HttpPost]
        public ActionResult UploadProfilePic(HttpPostedFileBase ProfilePic)
        {
            string fileName = Path.GetFileNameWithoutExtension(ProfilePic.FileName);
            string extension = Path.GetExtension(ProfilePic.FileName);
            fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            var ActiveStudentId = User.Identity.GetUserId();

            var ActiveStudentData = _db.Students.Where(s => s.StudentGuid == ActiveStudentId).FirstOrDefault();

            ActiveStudentData.ProfilePic = "../Content/" + fileName;
            fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
            ProfilePic.SaveAs(fileName);

            _db.SaveChanges();
            return RedirectToAction("Dashboard", "Student");
        }

        public ActionResult PurchaseHistory()
        {
            return View();
        }

        public ActionResult Fines()
        {
            return View(_db.Borrowers.Where(b => b.IsOverDue == true).ToList());
        }

        public ActionResult PayFines(Borrower _Borrower)
        {
            double TotalFineAmount = 200;
            ViewBag.TotalFineAmount = TotalFineAmount; 
            
            return View(_Borrower);
        }
    }
}