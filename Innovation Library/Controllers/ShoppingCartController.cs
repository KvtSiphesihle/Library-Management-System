using Innovation_Library.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Org.BouncyCastle.Asn1.X509;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class ShoppingCartController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        public void SendCouponEmail(double Discount)
        {
            var StudentId = User.Identity.GetUserId();
            Student _Student = _db.Students.Where(st => st.StudentGuid == StudentId).FirstOrDefault();

            string Subject = "Rewards & Loyalty Programmes";
            string Body = "Dear " + _Student.StudentName +
                "</br>" +
                "Congratutions you have been given " + Discount + " % for your next purchase" +
                "enjoy the rest of your day." +
                "</br>" +
                "Thank you" +
                "</br>" +
                "Kindly Regards" +
                "</br>" +
                "Innovation Library";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(_Student.Email));
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
        }
        public void GenerateCoapon(double Discount)
        {
            var guid = Guid.NewGuid().ToString();
            Coapon coapon = new Coapon
            {
                Discount = Discount,
                CoaponCode = guid.Substring(0, 5)
            };

            _db.Coapons.Add(coapon);
            _db.SaveChanges();
        }
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserDetails(string deliveryOption, double Total)
        {
            double deliveryFee = 0;
            Order order = new Order();

            if (deliveryOption == "Request Delivery")
            {
                deliveryFee += 60;
            }

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                OrderDetails orderProduct = new OrderDetails();
                Random OrderNoGenerator = new Random();
                Random random = new Random();
                var UserId = User.Identity.GetUserId();
                Student _Student = _db.Students.Where(s => s.StudentGuid == UserId).FirstOrDefault();

                var guid = Guid.NewGuid().ToString();

                var customerOrders = _db.Orders.Where(o => o.CustomerId == UserId).ToList().Count();

                var Discount = 0.0;

                switch (customerOrders)
                {
                    case 2:
                        Discount = 0.1;
                        GenerateCoapon(Discount);
                        SendCouponEmail(Discount);
                        break;
                    case 4:
                        Discount = 0.2;
                        GenerateCoapon(Discount);
                        SendCouponEmail(Discount);
                        break;
                    case 6:
                        Discount = 0.3;
                        GenerateCoapon(Discount);
                        SendCouponEmail(Discount);
                        break;
                    default:
                        break;
                }

                order.OrderNo = guid.Substring(0, 6).ToUpper(); 
                order.CustomerId = UserId;
                order.Total = Total + deliveryFee;
                order.CustomerId = _Student.StudentGuid;
                context.Orders.Add(order);
                context.SaveChanges();

                List<Cart> BookList = (List<Cart>)Session["cart"];
                List<CartView> Cart = new List<CartView>();

                if (BookList != null)
                {
                    foreach (var item in BookList)
                    {
                        CartView cartView = new CartView();
                        Book _Book = _db.Books.Where(x => x.BookId == item.pid).FirstOrDefault();
                        _Book.PurchaseCount += 1;
                        if (_Book.Quantity > 0)
                        {
                            _Book.Quantity -= 1;
                        }
                        _db.SaveChanges();
                        cartView.ProdId = _Book.BookId;
                        cartView.ProdName = _Book.BookTitle;
                        cartView.ProdImage = _Book.Image;
                        cartView.ProdPrice = Convert.ToDouble(_Book.BookPrice);
                        cartView.ProdQty = item.qty;
                        cartView.Total = item.qty * Convert.ToDecimal(_Book.BookPrice);
                        Cart.Add(cartView);
                    }

                    foreach (var item in Cart)
                    {
                        orderProduct.OrderId = order.OrderId;
                        orderProduct.BookId = item.ProdId;
                        orderProduct.ProductName = item.ProdName;
                        orderProduct.ProductPrice = Convert.ToDouble(item.ProdPrice);
                        orderProduct.Quantity = item.ProdQty;
                        _db.OrderDetails.Add(orderProduct);
                        _db.SaveChanges();
                    }
                    BookList.Clear();
                    Session["cart"] = null;

                    Session["OrderID"] = order.OrderId;
                    Session["OrderNo"] = order.OrderNo;
                }
            }

            return View();
        }
        [HttpPost]
        public ActionResult SaveUserDetails(Order order)
        {
            var OrderID = Convert.ToInt32(Session["OrderID"]);
            Order _order = _db.Orders.Find(OrderID);
            if (_order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _order.FirstName = order.FirstName;
            _order.LastName = order.LastName;
            _order.Email = User.Identity.Name;
            _order.Address = order.Address;
            _order.City = order.City;
            _order.Phone = order.Phone;
            _db.SaveChanges();
            return RedirectToAction("OrderPayment");
        }

        public ActionResult OrderPayment()
        {
            var OrderID = Convert.ToInt32(Session["OrderID"]);
            Order _order = _db.Orders.Find(OrderID);
            if (_order == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            return View(_order);
        }

        public ActionResult OrderPaymentSuccess()
        {
            var OrderID = Convert.ToInt32(Session["OrderID"]);
            Order order = _db.Orders.Find(OrderID);
            if (order != null)
            {
                order.isPayed = true;
                _db.SaveChanges();
            }
            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memoryStream);
            pdfDoc.Open();

            //Top Heading
            Chunk chunk = new Chunk("Order #" + order.OrderId + " Receipt.");
            pdfDoc.Add(chunk);

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);


            //Table
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            //0=Left, 1=Centre, 2=Right
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell no 1
            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Assets/images/logo.png"));
            image.ScaleAbsolute(150, 100);
            cell.AddElement(image);
            table.AddCell(cell);

            //Cell no 2
            chunk = new Chunk("Order No : #" + order.OrderId);
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(chunk);
            table.AddCell(cell);

            //Add table to document
            pdfDoc.Add(table);


            //Table
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 30f;
            table.SpacingAfter = 30f;
            table.DefaultCell.Padding = 4;

            //Cell
            cell = new PdfPCell(new Phrase("Order Details"));
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 8f;

            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("Product")));
            table.AddCell(new PdfPCell(new Phrase("Qty")));
            table.AddCell(new PdfPCell(new Phrase("Price")));

            foreach (var item in order.OrderDetails)
            {
                table.AddCell(new PdfPCell(new Phrase(item.Book.BookTitle)));
                table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString())));
                table.AddCell(new PdfPCell(new Phrase(item.ProductPrice.ToString())));
            }

            //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            //pdfDoc.Add(line);


            cell = new PdfPCell(new Phrase("Amount Due : R" + order.Total.ToString()));
            cell.Colspan = 3;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.PaddingBottom = 10f;
            cell.PaddingRight = 30f;
            table.AddCell(cell);

            pdfDoc.Add(table);

            Paragraph para = new Paragraph();
            para.Add("Hello " + order.FirstName + " " + order.LastName + ",\n\nThank you for choosing Innovation Library");

            pdfDoc.Add(para);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            memoryStream.Position = 0;

            var Body = "Your order has been processed successfully.";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(order.Email));
            msg.Subject = "Successful Order";
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(memoryStream, "OrderReport.pdf"));
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(msg);
            msg.Dispose();
            Session.Clear();
            return View();
        }

        public ActionResult ApplyCoupon(string couponCode)
        {
            var OrderID = Convert.ToInt32(Session["OrderID"]);
            Order _order = _db.Orders.Find(OrderID);

            var Discount = _db.Coapons.Where(d => d.CoaponCode == couponCode && d.IsExpired == false).FirstOrDefault();
            var InvalidDiscount = _db.Coapons.Where(d => d.CoaponCode == couponCode && d.IsExpired == true).FirstOrDefault();


            if (InvalidDiscount != null && Discount == null)
            {
                TempData["CouponStatus"] = "The coupon code has expired";
            }
            if (Discount == null && InvalidDiscount == null)
            {
                TempData["CouponStatus"] = "The coupon code is invalid";
            }
            else
            {
                if (_order != null && Discount != null && InvalidDiscount == null)
                {
                    var DiscountPrice = _order.Total * Discount.Discount;
                    var TotalAmtDue = _order.Total - DiscountPrice;
                    _order.Total = TotalAmtDue;
                    Discount.IsExpired = true;
                    _db.SaveChanges();
                    TempData["CouponStatus"] = "The coupon code has been applied";
                    Session["IsCouponApplied"] = true;
                }
            }

            return RedirectToAction("OrderPayment");
        }

        [HttpPost]
        public JsonResult AddToCart(int id)
        {
            Book _Product = _db.Books.Where(x => x.BookId == id).FirstOrDefault();

            if (Session["cart"] != null)
            {
                List<Cart> ProductList = (List<Cart>)Session["cart"];

                int check = 0;

                foreach (var item in ProductList)
                {


                    if (item.pid == id)
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
                    obj.pid = id;
                    obj.qty = 1;
                    ProductList.Add(obj);
                }

                Session["cart"] = (List<Cart>)ProductList;
            }
            else
            {
                List<Cart> Products = new List<Cart>();
                Cart obj = new Cart
                {
                    pid = id,
                    qty = 1
                };
                Products.Add(obj);
                Session["cart"] = Products;


            }
            List<Cart> Cart = (List<Cart>)Session["cart"];
            var Result = new
            {
                UserCart = Session["cart"],
                _ProductName = _Product.BookTitle,
                CartCount = Convert.ToInt32(Cart.Count)
            };

            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles ="Student")]
        public ActionResult Cart()
        {

            List<Cart> BookList = (List<Cart>)Session["cart"];
            List<CartView> Cart = new List<CartView>();

            if (BookList != null)
            {
                foreach (var item in BookList)
                {
                    CartView cartView = new CartView();
                    Book _Book = _db.Books.Where(x => x.BookId == item.pid).FirstOrDefault();
                    cartView.ProdId = _Book.BookId;
                    cartView.ProdName = _Book.BookTitle;
                    cartView.ProdImage = _Book.Image;
                    cartView.Author = _Book.Author;
                    cartView.ProdPrice = Convert.ToDouble(_Book.BookPrice);
                    cartView.ProdQty = item.qty;
                    cartView.Total = item.qty * Convert.ToDecimal(_Book.BookPrice);
                    Cart.Add(cartView);
                }
                ViewBag.CartCount = Cart.Count;
                return View(Cart);
            }
            return View();

        }

        //Increase Quantity
        
        public ActionResult IncrsQty(int id)
        {
            int Quantity = 0;
            List<Cart> ProductList = (List<Cart>)Session["cart"];
            for (int i = 0; i < ProductList.Count; i++)
            {
                if (ProductList[i].pid == id)
                {
                    ProductList[i].qty = ProductList[i].qty + 1;
                    Quantity = ProductList[i].qty;
                    break;
                }
            }
            Session["cart"] = (List<Cart>)ProductList;
            return Json(Quantity, JsonRequestBehavior.AllowGet);
        }

        //Decrease Quantity
        public ActionResult DecrsQty(int id)
        {
            int Quantity = 0;
            List<Cart> ProductList = (List<Cart>)Session["cart"];
            for (int i = 0; i < ProductList.Count; i++)
            {
                if (ProductList[i].pid == id)
                {
                    if (ProductList[i].qty > 1)
                    {
                        ProductList[i].qty = ProductList[i].qty - 1;
                        Quantity = ProductList[i].qty;
                    }
                    else
                    {
                        Quantity = 1;
                    }
                    break;
                }
            }
            Session["cart"] = (List<Cart>)ProductList;
            return Json(Quantity, JsonRequestBehavior.AllowGet);
        }

        //Remove Item From the Cart
        public ActionResult removeItem(int id)
        {
            List<Cart> ProductList = (List<Cart>)Session["cart"];

            if (ProductList.Count > 1)
            {
                for (int i = 0; i < ProductList.Count; i++)
                {
                    if (ProductList[i].pid == id)
                    {
                        ProductList.Remove(ProductList[i]);
                    }
                }
                Session["cart"] = (List<Cart>)ProductList;
            }
            else
            {
                ProductList.Clear();
                Session["cart"] = null;
            }

            return Json(Session["cart"] ?? "Empty");
        }
        [HttpPost]
        public JsonResult PlaceOrder(Order order, double TotalAmount)
        {
            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                OrderDetails orderProduct = new OrderDetails();
                Random OrderNoGenerator = new Random();
                Random random = new Random();

                var TrackingNo = "";

                for (int i = 0; i < 10; i++)
                {
                    var Tracking = random.Next(0, 9);
                    TrackingNo += Convert.ToString(Tracking);
                }
                var UserId = User.Identity.GetUserId();
                Student _Student = _db.Students.Where(s => s.StudentGuid == UserId).FirstOrDefault();

                order.CustomerId = UserId;
                order.Total = TotalAmount;
                order.FirstName = _Student.StudentName;
                order.Email = _Student.Email;
                order.CustomerId = _Student.StudentGuid;
                context.Orders.Add(order);
                context.SaveChanges();

                List<Cart> BookList = (List<Cart>)Session["cart"];
                List<CartView> Cart = new List<CartView>();

                if (BookList != null)
                {
                    foreach (var item in BookList)
                    {
                        CartView cartView = new CartView();
                        Book _Book = _db.Books.Where(x => x.BookId == item.pid).FirstOrDefault();
                        _Book.PurchaseCount += 1;
                        _db.SaveChanges();
                        cartView.ProdId = _Book.BookId;
                        cartView.ProdName = _Book.BookTitle;
                        cartView.ProdImage = _Book.Image;
                        cartView.ProdPrice = Convert.ToDouble(_Book.BookPrice);
                        cartView.ProdQty = item.qty;
                        cartView.Total = item.qty * Convert.ToDecimal(_Book.BookPrice);
                        Cart.Add(cartView);
                    }

                    foreach (var item in Cart)
                    {
                        orderProduct.OrderId = order.OrderId;
                        orderProduct.BookId = item.ProdId;
                        orderProduct.ProductPrice = Convert.ToDouble(item.ProdPrice);
                        orderProduct.Quantity = item.ProdQty;
                        _db.OrderDetails.Add(orderProduct);
                        _db.SaveChanges();
                    }
                    BookList.Clear();
                    Session["cart"] = null;

                    TempData["OrderNo"] = order.OrderId;
                }
            }
            var data = new
            {
                IsCompleted = true,
                OrderNumber = order.OrderId
            };

            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            MemoryStream memoryStream = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memoryStream);
            pdfDoc.Open();

            //Top Heading
            Chunk chunk = new Chunk("Order #" + order.OrderId + " Receipt.");
            pdfDoc.Add(chunk);

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);


            //Table
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            //0=Left, 1=Centre, 2=Right
            table.HorizontalAlignment = 0;
            table.SpacingBefore = 20f;
            table.SpacingAfter = 30f;

            //Cell no 1
            PdfPCell cell = new PdfPCell();
            cell.Border = 0;
            Image image = Image.GetInstance(Server.MapPath("~/Content/Images/logo.png"));
            image.ScaleAbsolute(150, 100);
            cell.AddElement(image);
            table.AddCell(cell);

            //Cell no 2
            chunk = new Chunk("Order No : #" + order.OrderId);
            cell = new PdfPCell();
            cell.Border = 0;
            cell.AddElement(chunk);
            table.AddCell(cell);

            //Add table to document
            pdfDoc.Add(table);


            //Table
            table = new PdfPTable(3);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 30f;
            table.SpacingAfter = 30f;
            table.DefaultCell.Padding = 4;

            //Cell
            cell = new PdfPCell(new Phrase("Order Details"));
            cell.Colspan = 3;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 8f;

            cell.BackgroundColor = BaseColor.LIGHT_GRAY;
            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("Product")));
            table.AddCell(new PdfPCell(new Phrase("Qty")));
            table.AddCell(new PdfPCell(new Phrase("Price")));

            foreach (var item in order.OrderDetails)
            {
                table.AddCell(new PdfPCell(new Phrase(item.Book.BookTitle)));
                table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString())));
                table.AddCell(new PdfPCell(new Phrase(item.ProductPrice.ToString())));
            }

            //line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            //pdfDoc.Add(line);


            cell = new PdfPCell(new Phrase("Amount Due : R" + TotalAmount));
            cell.Colspan = 3;
            cell.PaddingTop = 10f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.PaddingBottom = 10f;
            cell.PaddingRight = 30f;
            table.AddCell(cell);

            pdfDoc.Add(table);

            Paragraph para = new Paragraph();
            para.Add("Hello " + order.FirstName + " " + order.LastName + ",\n\nThank you for choosing Innovation Library");

            pdfDoc.Add(para);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            memoryStream.Position = 0;

            var Body = "Your order has been processed successfully.";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(order.Email));
            msg.Subject = "Successful Order";
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(memoryStream, "OrderReport.pdf"));
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(msg);
            msg.Dispose();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}