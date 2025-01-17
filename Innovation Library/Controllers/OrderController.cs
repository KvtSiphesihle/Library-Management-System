using Innovation_Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class OrderController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Order
        public ActionResult Index()
        {
            return View(_db.Orders.ToList());
        }
        // Details
        public ActionResult Details(int? id)
        {
            Order _order = _db.Orders.Find(id);
            if (_order == null)
            {
                return HttpNotFound();
            }
            return View(_order);
        }

        // Student Orders
        [Authorize(Roles ="Student")]
        public ActionResult UserOrders()
        {
            var stu_user_id = User.Identity.GetUserId();
            var orders = _db.Orders.Where(o => o.CustomerId == stu_user_id && o.IsCancelled == false).ToList();
            return View(orders);
        }

        // Order Tracking Center
        public ActionResult OrderTrackingCenter()
        {
            return View();
        }

        // Search Order
        [HttpPost]
        public ActionResult SearchValidOrder(string _OrderNumber)
        {
            var OrderNumber = _OrderNumber;

            if (OrderNumber == "")
            {
                TempData["Error"] = "Order number can't be empty.";
            }
            else
            {
                Order order = _db.Orders.Where(o => o.OrderNo == OrderNumber && o.isPayed == true).FirstOrDefault();
                if (order == null)
                {
                    TempData["Error"] = "Order has a pending payment.";
                }
                else
                {
                    Session["ValidOrder"] = true;
                    return View("OrderTrackingCenter", order);
                }
            }
            return RedirectToAction("OrderTrackingCenter");
        }

        public ActionResult ConfirmDelivery(int? id)
        {
            Order order = _db.Orders.Find(id);
            if (order == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            return View(order);
        }
        [HttpPost]
        public ActionResult ConfirmDelivery(int id)
        {
            Order order = _db.Orders.Find(id);
            if (order == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            order.IsDeliveryConfirmed = true;
            order.Status = "Delivered & Confirmed";
            _db.SaveChanges();
            return RedirectToAction("UserOrders");
        }
    }
}