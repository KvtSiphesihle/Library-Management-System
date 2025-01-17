using Innovation_Library.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace Innovation_Library.Controllers
{
    public class DriverController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public DriverController()
        {

        }

        public DriverController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: Driver
        public ActionResult Index()
        {
            return View(context.Drivers.ToList());
        }

        // GET: Driver
        public ActionResult RegisterDriver()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> RegisterDriver(Driver driver)
        {
            if (ModelState.IsValid)
            {
                var Password = "Driver@inno11";
                var user = new ApplicationUser { UserName = driver.EmailAddress, Email = driver.EmailAddress, ContactNo = driver.PhoneNumber, };
                var result = await UserManager.CreateAsync(user, Password);

                var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

                if (result.Succeeded)
                {
                    Driver _driver = new Driver();
                    _driver.EmailAddress = user.Email;
                    _driver.UserId = user.Id;
                    _driver.Name = driver.Name;
                    _driver.PhoneNumber = driver.PhoneNumber;

                    context.Drivers.Add(_driver);
                    context.SaveChanges();

                    if (!roleManager.RoleExists("Driver"))
                    {
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole
                        {
                            Name = "Driver"
                        };
                        roleManager.Create(role);
                        UserManager.AddToRole(user.Id, "Driver");
                    }
                    else
                    {
                        UserManager.AddToRole(user.Id, "Driver");
                    }
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "User Already Exist.");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AssignDuty(string UserId)
        {
            Driver Driver = null;
            if (UserId != null)
            {
                Session["DriverUserId"] = UserId;
                Driver = context.Drivers.Where(d => d.UserId == UserId).FirstOrDefault();
            }
            else
            {
                if (UserId == null)
                {
                    var DriverUserId = Session["DriverUserId"].ToString();
                    Driver = context.Drivers.Where(d => d.UserId == DriverUserId).FirstOrDefault();
                }
            }
            ViewBag.DriverName = Driver.Name;
            Session["DriverModel"] = Driver;
            var UndeliveredOrders = context.Orders.Where(o => o.isDelivered == false & o.AssignedDriverId == null).ToList();
            return View(UndeliveredOrders);
        }

        [HttpPost]
        public ActionResult ConfirmAssign(string OrderNumber, string driverId, string OrderValue)
        {
            Order _order = null;

            if (OrderNumber != "" && OrderValue != "")
            {
                TempData["Error"] = "You can't use select and input at the same time.";
                var UndeliveredOrders = context.Orders.Where(o => o.isDelivered == false & o.AssignedDriverId == null).ToList();
                return View("AssignDuty", UndeliveredOrders);
            }

            if (OrderNumber != "")
            {
                _order = context.Orders.Where(o => o.OrderNo == OrderNumber).FirstOrDefault();
            }
            if (OrderValue != "")
            {
                _order = context.Orders.Where(o => o.OrderNo == OrderValue).FirstOrDefault();
            }

            Driver _driver = context.Drivers.Where(o => o.UserId == driverId).FirstOrDefault();

            if (_order == null)
            {
                TempData["Error"] = "Request Failed! order not found.";
                var UndeliveredOrders = context.Orders.Where(o => o.isDelivered == false & o.AssignedDriverId == null).ToList();
                return View("AssignDuty", UndeliveredOrders);
            }

            if (_driver != null)
            {
                ViewBag.Name = _driver.Name;
            }
            ViewBag.OrderNo = _order.OrderNo;
            _order.AssignedDriverId = driverId;
            _order.IsAssigned = true;
            _driver.IsAvailable = false;
            context.SaveChanges();
            Session["DriverName"] = _driver.Name;
            Session["OrderNumber"] = _order.OrderNo;
            return RedirectToAction("AssignSuccess");
        }

        public ActionResult AssignSuccess()
        {
            return View();
        }

        // Deliveries
        [Authorize(Roles = "Driver")]
        public ActionResult Orders()
        {
            var UserId = User.Identity.GetUserId();
            return View(context.Orders.Where(o => o.AssignedDriverId == UserId));
        }
        public ActionResult ManageDuty(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order _order = context.Orders.Find(id);
            return View(_order);
        }

        [HttpPost]
        public ActionResult ActivatePickUp(int id)
        {
            Order _order = context.Orders.Find(id);
            if (_order == null)
            {
                return HttpNotFound();
            }
            _order.IsPicked = true;
            _order.Status = "Picked up";
            context.SaveChanges();
            return RedirectToAction("ManageDuty", new { id });
        }
        [HttpPost]
        public ActionResult ActivateOnRoute(int id)
        {
            Order _order = context.Orders.Find(id);
            if (_order == null)
            {
                return HttpNotFound();
            }
            _order.IsOnRoute = true;
            _order.Status = "On Route";
            context.SaveChanges();
            return RedirectToAction("ManageDuty", new { id });
        }
        [HttpPost]
        public ActionResult ActivateDelivered(int id)
        {
            Order _order = context.Orders.Find(id);
            // Driver driver = context.Drivers.Find(Convert.ToInt32(_order.AssignedDriverId));
            if (_order == null)
            {
                return HttpNotFound();
            }
            _order.isDelivered = true;
           // driver.IsAvailable = true;
            _order.Status = "Order Arrived";
            context.SaveChanges();
            return RedirectToAction("ManageDuty", new { id });
        }

        // Exchanges Deliveries
        [Authorize(Roles ="Driver")]
        public ActionResult DriverExchanges()
        {
            var UserId = User.Identity.GetUserId();
            return View(context.Exchanges.Where(ex => ex.DriverId == UserId).ToList());
        }
    }
}