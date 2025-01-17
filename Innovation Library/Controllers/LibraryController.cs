using Innovation_Library.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class LibraryController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Library

        public string GetUserId()
        {
            var StudentId = User.Identity.GetUserId();
            return StudentId;
        }
        [HttpPost]
        public ActionResult ExportReport()
        {
            var SignIns = _db.LibrarySignIns.ToList();

            string fontpath = Server.MapPath("~/Content/Assets/fonts/");

            BaseFont customfont = BaseFont.CreateFont(fontpath + "Kanit-Regular.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
            Font fontHeader = new Font(customfont, 14);

            //Light Font
            BaseFont Lightfont = BaseFont.CreateFont(fontpath + "Kanit-Regular.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
            Font fontText = new Font(Lightfont, 12);

            Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            //Top Heading
            Chunk chunk = new Chunk("Innovation Library Sign In's Report", fontHeader);
            pdfDoc.Add(chunk);

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);

            //Logo
            Image logo = Image.GetInstance(Server.MapPath("~/Content/Assets/images/logo.png"));
            logo.ScalePercent(15f);
            logo.SpacingBefore = 40f;
            logo.SpacingAfter = 60f;
            logo.Alignment = iTextSharp.text.Image.ALIGN_RIGHT;
            pdfDoc.Add(logo); ;

            //Table
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
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

            pdfDoc.Add(table);

            Paragraph para = new Paragraph();
            para.Font = fontText;
            para.Add("Generated on " + DateTime.Now.ToLongDateString());

            pdfDoc.Add(para);

            //Table
            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            table.AddCell(new PdfPCell(new Phrase("ID", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Email", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Name", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Time in", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Time out", fontText)));


            foreach (var item in SignIns)
            {
                table.AddCell(new PdfPCell(new Phrase(item.ID.ToString(), fontText)));
                table.AddCell(new PdfPCell(new Phrase(item.StudentEmail, fontText)));
                table.AddCell(new PdfPCell(new Phrase(item.Name, fontText)));
                table.AddCell(new PdfPCell(new Phrase(item.SignInTime, fontText)));
                table.AddCell(new PdfPCell(new Phrase(item.SignOutTime, fontText)));
            }

            pdfDoc.Add(table);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();

            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=sign-in-report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("LibrarySignIns");
        }

        public ActionResult LibrarySignIns()
        {
            return View(_db.LibrarySignIns.ToList());
        }

        public ActionResult IndexBorrowers()
        {
            return View(_db.Borrowers.ToList());
        }

        public ActionResult Index()
        {
            var TopSeller = _db.Books.Any(m => m.PurchaseCount > 5 && m.Quantity != 0);
            var TopSellerData = TopSeller ? _db.Books.Where(m => m.PurchaseCount > 5).FirstOrDefault() : _db.Books.Where(m => m.PurchaseCount < 5).FirstOrDefault();
            if (!TopSeller)
            {
                ViewBag.TopSeller = _db.Books.Where(b=>b.Quantity > 0).FirstOrDefault();
            }
            else
            {
                ViewBag.TopSeller = TopSellerData;
            }
            return View(_db.Books.ToList());
        }

        public ActionResult RequestBook()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RequestBook(BookRequest _Request)
        {
            if (ModelState.IsValid)
            {
                var StudentId = User.Identity.GetUserId();
                _Request.UserId = StudentId;
                _db.BookRequests.Add(_Request);
                _db.SaveChanges();
                return RedirectToAction("BookRequestSuccess");
            }

            return View();
        }
        //Book Request Success
        public ActionResult BookRequestSuccess()
        {
            return View();
        }

        //My Book Requests 
        [Authorize(Roles ="Student")]
        public ActionResult IndexRequests()
        {
            var StudentId = User.Identity.GetUserId();
            return View(_db.BookRequests.Where(b=>b.UserId == StudentId).ToList());
        }

        //Book Requests(Admin)
        [Authorize(Roles ="Admin")]
        public ActionResult BookRequests()
        {
            return View(_db.BookRequests.ToList());
        }

        public ActionResult Rooms()
        {
            return View(_db.Rooms.ToList());
        }

        public ActionResult ItemsIndex()
        {
            return View(_db.HireItems);
        }

        [HttpPost]
        public ActionResult SearchBook(string SearchText)
        {
            var TopSeller = _db.Books.Any(m => m.PurchaseCount > 5);
            var TopSellerData = TopSeller ? _db.Books.Where(m => m.PurchaseCount > 5).FirstOrDefault() : _db.Books.Where(m => m.PurchaseCount < 5).FirstOrDefault();
            ViewBag.TopSeller = TopSellerData;

            ApplicationDbContext entities = new ApplicationDbContext();
            var books = from c in entities.Books
                            where c.BookTitle.Contains(SearchText) || c.Author.Contains(SearchText)
                            select c;

            var SearchResult = books.ToList();

            return View("Index", SearchResult);
        }
        [HttpPost]
        public ActionResult BorrowBook(int id)
        {
            Borrower borrower = new Borrower();
            var Book = _db.Books.Find(id);
            var SavedBorrowerId = 0;

            if (Book == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var StudentId = User.Identity.GetUserId();
            var Student = _db.Students.Where(x => x.StudentGuid == StudentId).FirstOrDefault();
            if (Student == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            borrower.StudentId = Student.StudentGuid;
            borrower.StudentName = Student.StudentName;
            borrower.Email = Student.Email;
            borrower.ContactNo = Student.ContactNo;
            borrower.BorrowedDate = DateTime.Now;
            borrower.ExpectedReturnDate = DateTime.Now.AddMinutes(5);
            borrower.ReturnedDate = null;
            borrower.AllocatedBookId = id;
            borrower.BillStatus = "Incomplete";
            borrower.BookTitle = Book.BookTitle;
            borrower.Author = Book.Author;
            borrower.ISBN = Book.ISBN;
            Book.Quantity -= 1;
            _db.Borrowers.Add(borrower);
            _db.SaveChanges();

            SavedBorrowerId = borrower.BorrowerId;
            var res = Book;
            return RedirectToAction("BorrowSuccess", "Library", new { BorrowId = SavedBorrowerId });
        }

        public ActionResult BorrowSuccess(int? BorrowId)
        {
            Borrower _Borrower = _db.Borrowers.Find(BorrowId);
            if (_Borrower == null)
            {
                return HttpNotFound();
            }
            Book Book = _db.Books.Find(_Borrower.AllocatedBookId);
            if (Book == null)
            {
                return HttpNotFound();
            }
            ViewBag.ExpectedReturn = _Borrower.ExpectedReturnDate;
            return View(Book);
        }

        public ActionResult ReturnBook(int? id)
        {
            Borrower _Borrower = _db.Borrowers.Find(id);
            if (_Borrower != null)
            {
                DateTime ExpectedReturn = Convert.ToDateTime(_Borrower.ExpectedReturnDate);
                DateTime CurrentDate = DateTime.Now;

                if (CurrentDate > ExpectedReturn)
                {
                    _Borrower.IsOverDue = true;
                    _Borrower.BillAmount = 200;
                    return RedirectToAction("PayFines", "Student", _Borrower);
                }
            }
            else
            {
                return HttpNotFound();
            }

            return View(_Borrower);
        }

        [HttpPost]
        public ActionResult ReturnBook(int BookId, int BorrowerId)
        {
            Book _Book = _db.Books.Find(BookId);
            Borrower _Borrower = _db.Borrowers.Find(BorrowerId);

            if (_Borrower != null)
            {
                _Book.Quantity += 1;
                _Borrower.ReturnedDate = DateTime.Now.ToString();
                _Borrower.Status = "Returned";
                _Borrower.IsReturned = true;
                _db.SaveChanges();
            }
            return RedirectToAction("ReturnSuccess", "Library");
        }

        public ActionResult ReturnSuccess()
        {
            return View();
        }

        public ActionResult BorrowerDetails(int? id)
        {
            Borrower _Borrower = _db.Borrowers.Find(id);
            if (_Borrower == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Borrower);
        }

        public ActionResult AdminBorrowerDetails(int? id)
        {
            Borrower _Borrower = _db.Borrowers.Find(id);
            if (_Borrower == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Borrower);
        }
    }
}