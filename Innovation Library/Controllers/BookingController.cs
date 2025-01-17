using Aspose.BarCode.Generation;
using Innovation_Library.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNet.Identity;
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
    public class BookingController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Booking
        public ActionResult Index()
        {
            return View(_db.Bookings.ToList());
        }

        public ActionResult BookRoom(int? id)
        {
            Room _Room = _db.Rooms.Find(id);
            if (_Room == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Room);
        }

        [HttpPost]
        public ActionResult BookRoom(int? RoomId, Booking _Booking, BarCode barcode)
        {
            Room _Room = _db.Rooms.Find(RoomId);
            if (_Room == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var BookedSlot = _db.Bookings.Any(b => b.RoomId == RoomId && b.StartTime == _Booking.StartTime && b.StartDate == _Booking.StartDate);

            if (BookedSlot)
            {
                ViewBag.BookedSlot = "There is an on-going booking for this slot, try another slot";
                return View(_Room);
            }
            else
            {
                var UserId = User.Identity.GetUserId();
                Student _student = _db.Students.Where(s => s.StudentGuid == UserId).FirstOrDefault();
                _Booking.RoomId = Convert.ToInt32(RoomId);
                _Booking.StudentId = _student.StudentGuid;

                DateTime current = DateTime.Now;
                DateTime delayedTime = current.AddMinutes(5);
                _Booking.DelayTime = Convert.ToString(delayedTime);


                barcode.BarcodeType = BarcodeType.Code128;
                barcode.ImageType = Aspose.BarCode.Generation.BarCodeImageFormat.Png;
                barcode.Text = _Booking.SigningCode;

                string codeText = barcode.Text;
                string imageName = barcode.Text + "." + barcode.ImageType;

                string subPath = "~/Images";
                if (!System.IO.Directory.Exists(Server.MapPath(subPath)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                }

                string imagePath = "/Images/" + imageName;
                string imageServerPath = Server.MapPath("~" + imagePath);


                var encodeType = EncodeTypes.Code128;

                using (Stream str = new FileStream(imageServerPath, FileMode.Create, FileAccess.Write))
                {
                    BarcodeGenerator gen = new BarcodeGenerator(encodeType, codeText);
                    gen.Save(str, barcode.ImageType);
                }

                ViewBag.ImagePath = imagePath;
                ViewBag.ImageName = imageName;

                string fontpath = Server.MapPath("~/Content/Assets/fonts/");

                BaseFont customfont = BaseFont.CreateFont(fontpath + "Kanit-Regular.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
                Font fontHeader = new Font(customfont, 14);

                //Light Font
                BaseFont Lightfont = BaseFont.CreateFont(fontpath + "Kanit-Regular.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
                Font fontText = new Font(Lightfont, 12);

                Document pdfDoc = new Document(PageSize.A4, 25, 25, 25, 15);
                MemoryStream memoryStream = new MemoryStream();
                PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, memoryStream);

                pdfDoc.Open();



                //Top Heading
                Chunk chunk = new Chunk("Room Booking Report", fontHeader);
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
                para.Add("Hello " + _student.StudentName +"\nRoom booking is successfully, your sign in time will be delayed by 10 mins\n" +
                    "If you don't sign in before the delay time ends your slot will be automatically cancelled.\n");

                pdfDoc.Add(para);


                //Details Heading
                Chunk detailsHeading = new Chunk("\nSigning unique code : " + _Booking.SigningCode , fontHeader);
                pdfDoc.Add(detailsHeading);


                //Table
                table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;

                table.SpacingBefore = 10f;
                table.SpacingAfter = 10f;
                table.DefaultCell.Padding = 4;

                table.AddCell(new PdfPCell(new Phrase("Student name", fontText)));
                table.AddCell(new PdfPCell(new Phrase("Unique Signing Code", fontText)));
                table.AddCell(new PdfPCell(new Phrase("Start Date", fontText)));
                table.AddCell(new PdfPCell(new Phrase("Time", fontText)));
                //table.AddCell(new PdfPCell(new Phrase("Time out", fontText)));


                table.AddCell(new PdfPCell(new Phrase(_student.StudentName, fontText)));
                table.AddCell(new PdfPCell(new Phrase(_Booking.SigningCode, fontText)));
                table.AddCell(new PdfPCell(new Phrase(_Booking.StartDate, fontText)));
                table.AddCell(new PdfPCell(new Phrase(_Booking.StartTime, fontText)));
                //table.AddCell(new PdfPCell(new Phrase(_Booking.EndTime, fontText)));

                pdfDoc.Add(table);

                // User Details Table
                table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;

                table.SpacingBefore = 20f;
                table.SpacingAfter = 10f;
                table.DefaultCell.Padding = 4;

                //Cell header
                Chunk UserDetailsHeader = new Chunk("Student Info.", fontHeader);
                pdfDoc.Add(UserDetailsHeader);

                //Data Cells
                table.AddCell(new PdfPCell(new Phrase("Email", fontText)));
                table.AddCell(new PdfPCell(new Phrase(_student.Email, fontText)));
                table.AddCell(new PdfPCell(new Phrase("Full name", fontText)));
                table.AddCell(new PdfPCell(new Phrase(_student.StudentName, fontText)));
                pdfDoc.Add(table);

                //Image Note
                Paragraph imageNote = new Paragraph();
                imageNote.Font = fontText;
                imageNote.SpacingBefore = 30f;
                imageNote.Add("Scan the bar code with camera or crop the bar code and upload it to the scanner.");
                pdfDoc.Add(imageNote);

                //Add Barcode Image
                Image barcodeImg = Image.GetInstance(Server.MapPath(imagePath));
                barcodeImg.ScalePercent(40f);
                barcodeImg.SpacingBefore = 20f;
                barcodeImg.SpacingAfter = 10f;
                pdfDoc.Add(barcodeImg); ;

                //Outro
                Paragraph Outro = new Paragraph();
                Outro.Font = fontText;
                Outro.Add("Thank you for choosing Innovation Library.\nRegards Management Team");
                pdfDoc.Add(Outro);


                pdfWriter.CloseStream = false;
                pdfDoc.Close();

                memoryStream.Position = 0;

                string Subject = "Booking Request Success";
                string Body = "Thank you for choosing Innovation Library " + _student.StudentName + "\n" +
                    "Please find the attatched document for the copy of your room booking report\n\n" +
                    "Kindly Regards\nInnovation Library";

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
                msg.To.Add(new MailAddress(_student.Email));
                msg.Subject = Subject;
                msg.Body = Body;
                msg.IsBodyHtml = true;
                msg.Attachments.Add(new Attachment(memoryStream, "Booking-report.pdf"));
                SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                smtp.Timeout = 1000000;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;
                smtp.Send(msg);
                msg.Dispose();
                _db.Bookings.Add(_Booking);
                _db.SaveChanges();
            }
            return RedirectToAction("RoomBookingSuccess");
        }

        public ActionResult RoomBookingSuccess()
        {
            return View();
        }

        public ActionResult Bookings()
        {
            return View(_db.Bookings.ToList());
        }

        public JsonResult CancelBooking(int? id)
        {
            Booking _booking = _db.Bookings.Find(id);
            _booking.Status = "Cancelled";
            _db.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
    }
}