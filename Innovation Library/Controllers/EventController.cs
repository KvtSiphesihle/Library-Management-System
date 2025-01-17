using Innovation_Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class EventController : Controller
    {
        ApplicationDbContext __db = new ApplicationDbContext();
        // GET: Event
        [Authorize(Roles="Student")]
        public ActionResult Index()
        {
            return View(__db.Events.OrderByDescending(i=>i.EventID).ToList());
        }
        public ActionResult EventsIndex()
        {
            return View(__db.Events.ToList());
        }
        public ActionResult HostEvent()
        {
            return View();
        }
        [HttpPost]
        public ActionResult HostEvent(Event e)
        {
            __db.Events.Add(e);
            __db.SaveChanges();

            Ticket __ticket = new Ticket();
            
            for (int i = 0; i < e.NumberOfSeats; i++)
            {
                var __str = "";
                Random __rand = new Random();
                for (int j = 0; j < 3; j++)
                {
                    __str += __rand.Next(1, 9);
                }

                __ticket.Address = e.Location;
                __ticket.Date = e.FullDate.ToLongDateString();
                __ticket.Time = e.FullDate.ToShortTimeString();
                __ticket.TicketNumber = i.ToString() + __str;
                __ticket.EventID = e.EventID;
                __db.Tickets.Add(__ticket);
                __db.SaveChanges();
            }
            return RedirectToAction("EventHostingSuccess");
        }
        public ActionResult ReserveTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Event __event = __db.Events.Find(id);
            if (__event == null)
            {
                return HttpNotFound();
            }
            return View(__event);
        }
        [HttpPost]
        public ActionResult ReserveTicket(int id)
        {
            Event __event = __db.Events.Find(id);
            var __userID = User.Identity.GetUserId();
            Ticket __ticket = __event.Tickets.Where(_t => _t.IsSoldOut == false).First();
            __ticket.UserID = __userID;
            __ticket.IsSoldOut = true;

            Student __student = __db.Students.Where(st => st.StudentGuid == __userID).First();
            Participant __participant = new Participant();
            __participant.EventID = __event.EventID;
            __participant.UserID = __student.StudentGuid;
            bool Is__Participant__Exist = __db.Participants.Any(t => t.UserID == __participant.UserID);

            if (Is__Participant__Exist)
            {
                TempData["Error"] = "You have already reserved !";
                return RedirectToAction("ReserveTicket", new { id });
            }
            else
            {
                __event.Participants.Add(__participant);
            }
            
            __db.SaveChanges();
            return RedirectToAction("ReserveSuccess");
        }
        public ActionResult ReserveSuccess()
        {
            return View();
        }
        public ActionResult Terminate(int? id)
        {
            return View();
        }
        public ActionResult EventHostingSuccess()
        {
            return View();
        }
    }
}