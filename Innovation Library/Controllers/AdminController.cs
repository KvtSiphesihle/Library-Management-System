using Innovation_Library.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApproveRequest(int? id)
        {
            Borrower _Borrower = _db.Borrowers.Find(id);
            if (_Borrower == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Borrower);
        }

        [HttpPost]
        public ActionResult ApproveRequest(int id)
        {
            Borrower _Borrower = _db.Borrowers.Find(id);
            if (_Borrower == null)
            {
                return HttpNotFound();
            }
            _Borrower.IsAccepted = true;
            _Borrower.Status = "Accepted";
            _db.SaveChanges();
            return RedirectToAction("IndexBorrowers", "Library");
        }

        public ActionResult RoomIndex()
        {
            return View(_db.Rooms.ToList());
        }

        public ActionResult ScanRefCard()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult AcceptHireRequest(string RefNo)
        {
            
            var _hiring = _db.Hirings.Where(h => h.ReferenceNo == RefNo).FirstOrDefault();
            if (_hiring == null)
            {
                var _booking = _db.Bookings.Where(b=>b.SigningCode == RefNo).FirstOrDefault();
                if (_booking == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                DateTime CurrentDate = DateTime.Now;

                if (CurrentDate > Convert.ToDateTime(_booking.DelayTime) && !_booking.IsSignedIn)
                {
                    return RedirectToAction("SessionExpired", new { id = _booking.BookingId});
                }
                else
                {
                    return RedirectToAction("SignInDetails", new { id = _booking.BookingId });
                }   
            }

            var ID = _hiring.HiringId;
            if (!_hiring.IsPayed)
            {
                return RedirectToAction("PendingPayment", new { id = ID});
            }
            _hiring.IsAccepted = true;
            _db.SaveChanges();
            return RedirectToAction("HireAcceptRequest", new { id = ID});
        }

        public ActionResult SignInDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking _booking = _db.Bookings.Find(id);
            var User_ID = _booking.StudentId;
            var _student = _db.Students.Where(s=>s.StudentGuid == User_ID).FirstOrDefault();
            if (_booking == null)
            {
                return HttpNotFound();
            }
   
            DateTime CurrentDate = DateTime.Now;

            LibrarySignIn _librarySignIn = new LibrarySignIn();

            int Minutes = 60;
            int InMinutes = CurrentDate.Minute;
            int Hour = DateTime.Now.Hour;
            int Seconds = DateTime.Now.Second;
         
            string SignInFullTime = CurrentDate.ToShortTimeString();
            string SigOutFullTime = Convert.ToDateTime(SignInFullTime).AddMinutes(Minutes).ToShortTimeString();

            _librarySignIn.Name = _student.StudentName;
            _librarySignIn.StudentEmail = _student.Email;
            _librarySignIn.SignInTime = SignInFullTime;
            _librarySignIn.SignOutTime = SigOutFullTime;
            _librarySignIn.BookingID = _booking.BookingId;

            var SignInKey = _librarySignIn.SignInKey;
            var Name = _librarySignIn.Name;
            var Email = _librarySignIn.StudentEmail;
            var TimeIn = _librarySignIn.SignInTime;
            var TimeOut = _librarySignIn.SignOutTime;
            var BookingID = _booking.BookingId;

            Session["IsTimeOut"] = false;
            Session["status"] = "Eligible For Sign in";

            if (!_db.LibrarySignIns.Any(s=>s.BookingID == BookingID))
            {
                _booking.IsSignedIn = true;
                _db.LibrarySignIns.Add(_librarySignIn);
                _db.SaveChanges();
            }
            else if(_db.LibrarySignIns.Any(s => s.BookingID == BookingID))
            {
                LibrarySignIn librarySignIn = _db.LibrarySignIns.Where(s => s.BookingID == BookingID).FirstOrDefault();
                var _TimeOut = Convert.ToDateTime(librarySignIn.SignOutTime);
                var _Current = DateTime.Now;

                if (_Current > _TimeOut)
                {
                    Session["status"] = "Session Time out";
                    Session["IsTimeOut"] = true;
                    librarySignIn.Status = "Time out";
                    ViewBag.Ongoing = _librarySignIn.Name + " has already signed in, and the time is out for this session.";
                    _db.SaveChanges();
                }
                else
                {
                    ViewBag.Ongoing = _librarySignIn.Name + " has already signed in, and the session is still on-going right now.";
                }
                return View(librarySignIn);
            }          
            return View(_librarySignIn);
        }

        public ActionResult SessionExpired(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking _booking = _db.Bookings.Find(id);
            ViewBag.ID = id;
            return View(_booking);
        }

        public ActionResult PendingPayment(int? id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_hiring);
        }

        public ActionResult HireAcceptRequest(int? id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_hiring);
        }
        [HttpPost]
        public ActionResult RequestPayment(int? id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring ==  null)
            {
                return HttpNotFound();
            }
            Student _student = _db.Students.Where(s => s.StudentGuid == _hiring.StudentId).FirstOrDefault();
            var StudentEmail = _student.Email;
            string Subject = "Payment Request For Hiring";
            string Body = "Dear " + _student.StudentName + " you're requested to complete the payment for your requested item hire\n" +
                "<br />" +
                "<h5>Hire Item details</h5>" +
                "<br />" +
                "<table border=\"1\">" +
                "<tr>" +
                "<th>Hire Date</th>" +
                "<th>Reference No</th>" +
                "<th>No Of days</th>" +
                "<th>Bill Status</th>" +
                "<th>Hire Total Amount</th>" +
                "</tr>" +
                "<tr>" +
                "<td>" + _hiring.HireDate + "</td>" +
                "<td><b>" + _hiring.ReferenceNo + "</b></td>" +
                "<td><b>" + _hiring.NoOfDays + "</b></td>" +
                "<td>" + _hiring.BillStatus + "</td>" +
                "<td>" + _hiring.HireTotalAmount + "</td>" +
                "</tr>" +
                "</table>" +
                "\n" +
                "Kindly Regards \n<b>Innovation Library</b>";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(StudentEmail));
            msg.Subject = Subject;
            msg.Body = Body;
            msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(msg);
            msg.Dispose();

            return RedirectToAction("PaymentRequestSuccess");
        }

        public ActionResult PaymentRequestSuccess()
        {
            return View();
        }
    }
}