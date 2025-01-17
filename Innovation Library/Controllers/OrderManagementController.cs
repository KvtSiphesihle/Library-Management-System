using Innovation_Library.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class OrderManagementController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();

        // GET: OrderManagement
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExchangeItem()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ExchangeItem(string OrderNo)
        {
            var ORDER_Identity = OrderNo;
            Order order = context.Orders.Where(o => o.OrderNo == ORDER_Identity).FirstOrDefault();
            if (order != null)
            {
                Session["ItemCount"] = order.OrderDetails.Count();
                Session["ValidOrder"] = order;
                return RedirectToAction("ValidOrderItems");
            }
            ViewBag.Error = "Order Not Found";
            return View();
        }

        //Items In Order
        public ActionResult ValidOrderItems()
        {
            ViewBag.ItemCount = Session["ItemCount"];
            var order = Session["ValidOrder"] as Order;
            return View(order);
        }
        //Choose Return Type (Refund or Replace)
        public ActionResult ExchangeType(int? id, int? itemID)
        {
            Session["order_id"] = id;
            Session["item_id"] = itemID;
            if (itemID == null)
            {
                Session["NewOrderRequest"] = true;
            }
            return View();
        }
        [HttpPost]
        public ActionResult ExchangeType(int? order_ID, int? item_ID, string ExchangeOption, string ExchangeReason, string ExchangeOtherReason)
        {
            //int order_ID, int item_ID
            Order order = context.Orders.Find(order_ID);
            Session["RefundAmount"] = order.Total;
            if (!order_ID.Equals(null) && !item_ID.Equals(null))
            {
                Book product = context.Books.Find(item_ID);
                ItemExchange itemExchange = new ItemExchange();
                itemExchange.ItemID = product.BookId;
                itemExchange.ItemPrice = Convert.ToDouble(product.BookPrice);
                itemExchange.ExchangeOtherReason = ExchangeOtherReason;
                itemExchange.OrderNo = order.OrderNo;
                itemExchange.ExchangeType = ExchangeOption;
                itemExchange.ExchangeReason = ExchangeReason;
                itemExchange.ItemName = product.BookTitle;

                context.ItemExchanges.Add(itemExchange);
                context.SaveChanges();
                if (ExchangeOption == "Replace Item")
                {
                    return RedirectToAction("ChooseItem", new { id = itemExchange.ItemExchangeID });
                }
            }
            else
            {
                if (!order_ID.Equals(null) && item_ID.Equals(null))
                {
                    order.Status = "Cancelled By Admin";
                    order.IsCancelled = true;
                    TempData["NewOrderRequestMessage"] = "You are requested to add new order.";

                    OrderExchange _OrderExchange = new OrderExchange
                    {
                        ExchangeOtherReason = ExchangeOtherReason,
                        ExchangeType = ExchangeOption,
                        OrderId = order.OrderId,
                        ExchangeReason = ExchangeReason
                    };

                    context.OrderExchanges.Add(_OrderExchange);
                    context.SaveChanges();
                    if (ExchangeOption == "Replace Order")
                    {
                        return RedirectToAction("Index", "Library");
                    }
                }
            }
            return RedirectToAction("Refund");
        }

        //Refund 
        public ActionResult Refunds()
        {
            return View(context.Refunds.ToList());
        }

        public ActionResult Refund()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Refund(Refund _refund)
        {
            var UserId = User.Identity.GetUserId();
            double TotalAmount = Convert.ToInt32(Session["RefundAmount"]);
            _refund.RefundAmount = TotalAmount;
            _refund.UserID = UserId;
            context.Refunds.Add(_refund);
            context.SaveChanges();
            return RedirectToAction("RefundSuccess");
        }

        public ActionResult RefundSuccess()
        {
            return View();
        }


        //Exchange Order
        public ActionResult ChooseItem(int? id)
        {
            ItemExchange itemExchange = context.ItemExchanges.Find(id);
            if (!itemExchange.Equals(null))
            {
                ViewBag.ItemExchange = itemExchange;
                Session["itemExchange"] = itemExchange;
                Order order = context.Orders.Where(o => o.OrderNo == itemExchange.OrderNo).FirstOrDefault();
                Session["Order"] = order;
            }
            return View(context.Books.ToList());
        }

        //Item Selected
        public ActionResult ProcessExchange(int? newItemId, int? oldItemId, string orderNo)
        {
            if (newItemId.Equals(null) || oldItemId.Equals(null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Book newItem = context.Books.Find(newItemId);
            Book oldItem = context.Books.Find(oldItemId);
            Order order = context.Orders.Where(o => o.OrderNo == orderNo).FirstOrDefault();

            dynamic Products = new ExpandoObject();
            Session["newItemModel"] = newItem;
            Session["oldItemModel"] = oldItem;
            Session["Order"] = order;

            return View(Products);
        }

        //Item Checkout
        public ActionResult ItemCheckout(int? orderID, int? newID, int? oldID, double totalAmount)
        {
            if (orderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = context.Orders.Find(orderID);
            if (order == null)
            {
                return HttpNotFound();
            }
            Book product = context.Books.Find(newID);
            Session["orderID"] = order.OrderId;
            Session["newItemID"] = newID;
            Session["oldItemID"] = oldID;
            Session["totalAmount"] = totalAmount;
            return View(product);
        }

        //Checkout Successful
        public ActionResult CheckoutSuccess()
        {
            var orderID = Session["orderID"];
            var newID = Session["newItemID"];
            var oldID = Session["oldItemID"];
            double TotalAmount = Convert.ToDouble(Session["totalAmount"]);

            Order order = context.Orders.Find(orderID);
            Book newProduct = context.Books.Find(newID);
            Book oldProduct = context.Books.Find(oldID);

            if (Session["exchange"] != null)
            {
                List<Cart> ProductList = (List<Cart>)Session["exchange"];

                int check = 0;

                foreach (var item in ProductList)
                {
                    if (item.pid == newProduct.BookId)
                    {
                        item.qty = item.qty + 1;
                        check = 0;
                        break;
                    }
                    else
                    {
                        check = 1;
                    }
                }
                if (check == 1)
                {
                    Cart obj = new Cart();
                    obj.pid = newProduct.BookId;
                    obj.qty = 1;
                    ProductList.Add(obj);
                }

                Session["exchange"] = (List<Cart>)ProductList;
            }
            else
            {
                List<Cart> Products = new List<Cart>();
                Cart obj = new Cart
                {
                    pid = newProduct.BookId,
                    qty = 1
                };
                Products.Add(obj);
                Session["exchange"] = Products;
            }

            List<Cart> BookList = (List<Cart>)Session["exchange"];

            if (newProduct == null || order == null || oldProduct == null)
                return HttpNotFound();

            var orderRemoveItem = order.OrderDetails.Where(x => x.BookId == oldProduct.BookId && x.OrderId == order.OrderId).FirstOrDefault();

            context.OrderDetails.Remove(orderRemoveItem);

            OrderDetails _orderDetails = new OrderDetails();

            _orderDetails.BookId = newProduct.BookId;
            _orderDetails.ProductPrice = Convert.ToDouble(newProduct.BookPrice);
            _orderDetails.Quantity = 1;
            _orderDetails.OrderId = order.OrderId;

            context.OrderDetails.Add(_orderDetails);

            ReturnItems orderDetails = new ReturnItems();

            Exchanges _Exchanges = new Exchanges();
            _Exchanges.UserId = User.Identity.GetUserId();
            _Exchanges.OrderId = order.OrderId;
            _Exchanges.Status = "OK";
            _Exchanges.ExchangeTime = DateTime.Now.ToShortTimeString();
            _Exchanges.AmountDue = TotalAmount;
            context.Exchanges.Add(_Exchanges);
            context.SaveChanges();

            foreach (var item in BookList)
            {
                Book _Book = context.Books.Where(x => x.BookId == item.pid).FirstOrDefault();
                orderDetails.BookId = newProduct.BookId;
                orderDetails.ExchangeId = _Exchanges.ExchangeId;
                context.ReturnItems.Add(orderDetails);
                context.SaveChanges();
            }
            return View();
        }

        //Complete Item Exchange
        public ActionResult ItemExchangeCompleted(int? orderID, int? newID, int? oldID, double oweAmount)
        {
            if (orderID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = context.Orders.Find(orderID);
            Book newProduct = context.Books.Find(newID);
            Book oldProduct = context.Books.Find(oldID);
            if (newProduct == null || order == null || oldProduct == null)
                return HttpNotFound();

            var orderRemoveItem = order.OrderDetails.Where(x => x.BookId == oldProduct.BookId && x.OrderId == order.OrderId).FirstOrDefault();
            context.OrderDetails.Remove(orderRemoveItem);

            OrderDetails orderDetails = new OrderDetails();

            orderDetails.BookId = newProduct.BookId;
            orderDetails.ProductPrice = Convert.ToDouble(newProduct.BookPrice);
            orderDetails.Quantity = 1;
            orderDetails.OrderId = order.OrderId;

            //context.SaveChanges();

            context.OrderDetails.Add(orderDetails);
            context.SaveChanges();
            return RedirectToAction("ExchangeSuccess");
        }

        //Exchange Success
        public ActionResult ExchangeSuccess()
        {
            return View();
        }

        // Exchanges
        public ActionResult Exchanges()
        {
            return View(context.Exchanges.ToList());
        }

        // Exchange Details
        public ActionResult ExchangeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            return View(_Exchanges);
        }

        // Assign Exchange Duty
        public ActionResult AssignExchangeDuty(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            return View(_Exchanges);
        }
        [HttpPost]
        public ActionResult AssignExchangeDuty(int id, string DriverId)
        {
            Exchanges _Exchanges = context.Exchanges.Find(id);
            Driver _Driver = context.Drivers.Where(d => d.UserId == DriverId).First();
            _Exchanges.DriverId = DriverId;
            context.SaveChanges();
            return View("Exchanges");
        }

        // Driver Exchange Details
        public ActionResult DriverExchangeDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            return View(_Exchanges);
        }

        // Pickup Item
        public ActionResult PickUp(int id)
        {
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            _Exchanges.Status = "Exchange Item(s) has been Picked";
            _Exchanges.IsPickedUp = true;
            context.SaveChanges();
            return View("DriverExchangeDetails", _Exchanges);
        }

        // Notify Driver
        public ActionResult NotifyCustomer(int id)
        {
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            _Exchanges.Status = "Exchange Item(s) has Arrived";
            _Exchanges.IsArrived = true;
            context.SaveChanges();
            return View("DriverExchangeDetails", _Exchanges);
        }

        // Customer Item Exchanges
        [Authorize(Roles ="Student")]
        public ActionResult MyExchanges()
        {
            var UserId = User.Identity.GetUserId();
            return View(context.Exchanges.Where(ex => ex.UserId == UserId).ToList());
        }

        // Confirm Collection
        public ActionResult ConfirmCollection(int? id)
        {
            Exchanges _Exchanges = context.Exchanges.Find(id);
            if (_Exchanges == null)
            {
                return HttpNotFound();
            }
            _Exchanges.Status = "Collected";
            _Exchanges.IsCollected = true;
            context.SaveChanges();
            return RedirectToAction("MyExchanges");
        }

        // Order Exchanges
        public ActionResult OrderExchangeReports()
        {
            return View(context.OrderExchanges.ToList());
        }

        // Schedule Order
        public ActionResult ScheduleOrder()
        {
            List<Order> orders = context.Orders.ToList();
            return View(orders);
        }
        [HttpPost]
        public ActionResult ScheduleOrder(OrderSchedule orderSchedule)
        {
            Order order = context.Orders.Where(o => o.OrderNo == orderSchedule.OrderNumber).FirstOrDefault();
            if (order == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            order.ArrivalDate = Convert.ToDateTime(order.ArrivalDate).AddMinutes(orderSchedule.Days).ToShortDateString();
            context.SaveChanges();
            return RedirectToAction("ValidOrderItems");
        }

        // Items Exchanges
        public ActionResult ItemExchangeReports()
        {
            return View(context.ItemExchanges.ToList());
        }
    }
}