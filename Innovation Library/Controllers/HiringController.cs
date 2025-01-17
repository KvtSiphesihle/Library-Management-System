using Aspose.BarCode.Generation;
using Innovation_Library.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using static iTextSharp.text.pdf.AcroFields;

namespace Innovation_Library.Controllers
{
    public class HiringController : Controller
    {
        ApplicationDbContext _db = new ApplicationDbContext();

        // GET: Hiring

        public ActionResult Index()
        {
            return View(_db.HireItems.ToList());
        }

        //Hirings
        public ActionResult Hirings()
        {
            return View(_db.Hirings.ToList());
        }

        //My Hiring History
        public ActionResult HiringHistory()
        {
            var StudentID = User.Identity.GetUserId();
            var _hiring = _db.Hirings.Where(h => h.StudentId == StudentID).ToList();
            return View(_hiring);
        }
        public ActionResult NewHireItem()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewHireItem(HireItems _HireItems)
        {
            if (ModelState.IsValid)
            {
                string fileName = Path.GetFileNameWithoutExtension(_HireItems.ImageFile.FileName);
                string extension = Path.GetExtension(_HireItems.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                _HireItems.Image = "../Content/" + fileName;
                fileName = Path.Combine(Server.MapPath("../Content/"), fileName);
                _HireItems.ImageFile.SaveAs(fileName);
                _db.HireItems.Add(_HireItems);
                _db.SaveChanges();
            }
            return RedirectToAction("ItemsIndex", "Library");
        }
        public ActionResult TermsConditions(int? id)
        {
            HireItems _Item = _db.HireItems.Find(id);
            if (_Item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Item);
        }
        [HttpPost]
        public ActionResult AgreeTerms(int id)
        {
            var StudentId = User.Identity.GetUserId();
            var Student = _db.Users.Where(u => u.Id == StudentId).FirstOrDefault();
            var User_email = User.Identity.Name;
            return RedirectToAction("HireItem", new {Id = id});
        }

        public ActionResult HireItem(int? Id, string Terms)
        {
            HireItems _Item = _db.HireItems.Find(Id);
            if (_Item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(_Item);
        }

        [HttpPost]
        public ActionResult HireItemDetails(Hiring _Hiring, int id)
        {
            HireItems _Item = _db.HireItems.Find(id);
            if (_Item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var StuId = User.Identity.GetUserId();
            DateTime Date = DateTime.Now;

            _Hiring.StudentId = StuId;
            _Hiring.ExpectedReturn = Date.AddDays(Convert.ToInt32(_Hiring.NoOfDays)).ToShortDateString();
            _Hiring.HireTotalAmount = _Item.ItemPrice * int.Parse(_Hiring.NoOfDays);
            _Hiring.ItemId = id;
            _Hiring.StudentEmail = User.Identity.Name;

            ViewBag.UserId = StuId;
            ViewBag.ItemId = id;
            ViewBag.ItemName = _Item.ItemName;
            ViewBag.Price = _Item.ItemPrice;
            ViewBag.Image = _Item.Image;

            _db.Hirings.Add(_Hiring);
            _db.SaveChanges();

            ViewBag.HiringId = _Hiring.HiringId;

            return View(_Hiring);
        }

        [HttpPost]
        public ActionResult ConfirmItemHire(int? HireId)
        {
            Hiring _Item = _db.Hirings.Find(HireId);

            HireItems HireItem = _db.HireItems.Find(_Item.ItemId);

            if (_Item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            HireItem.ItemQuantity -= 1;

            _db.SaveChanges();

            return RedirectToAction("Hired", "Hiring", new { HireId = _Item.HiringId});
        }

        public ActionResult HiringError()
        {
            return View();
        }

        public ActionResult Hired(int? HireId)
        {
            Hiring _Item = _db.Hirings.Find(HireId);
            HireItems HireItem = _db.HireItems.Find(_Item.ItemId);
            ViewBag.Image = HireItem.Image;
            ViewBag.ItemName = HireItem.ItemName;
            Session["HireId"] = HireId;
            return View(_Item);
        }

        [HttpPost]
        public ActionResult GenerateHireReport(int? id, BarCode barcode)
        {
            Hiring _Item = _db.Hirings.Find(id);
            if (_Item == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            barcode.BarcodeType = BarcodeType.Code128;
            barcode.ImageType = Aspose.BarCode.Generation.BarCodeImageFormat.Png;
            barcode.Text = _Item.ReferenceNo;

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
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
            pdfDoc.Open();

            //Top Heading
            Chunk chunk = new Chunk("Item Hire Reference Card", fontHeader);
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
            para.Add("Hello Customer\n\nYour item hire request has been processed, please do present this doc during item collection.\n");

            pdfDoc.Add(para);


            //Details Heading
            Chunk detailsHeading = new Chunk("\nReference No : " + _Item.ReferenceNo, fontHeader);
            pdfDoc.Add(detailsHeading);

            Paragraph para2 = new Paragraph();
            para.Font = fontText;
            para.Add("Full details");

            pdfDoc.Add(para2);

            //Table
            table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell
            cell = new PdfPCell(new Phrase("Item Hire Details", fontHeader));
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.PaddingBottom = 8f;

            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("Reference Code", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Bill Status", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Total price", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Hire date", fontText)));


            table.AddCell(new PdfPCell(new Phrase(_Item.ReferenceNo, fontText)));
            table.AddCell(new PdfPCell(new Phrase(_Item.BillStatus, fontText)));
            table.AddCell(new PdfPCell(new Phrase("R" + _Item.HireTotalAmount.ToString(), fontText)));
            table.AddCell(new PdfPCell(new Phrase(_Item.HireDate, fontText)));

            pdfDoc.Add(table);

            // User Details Table
            var StudentId = _Item.StudentId;
            var Student = _db.Users.Where(u => u.Id == StudentId).FirstOrDefault();
            if (Student != null)
            {
                table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;

                table.SpacingBefore = 20f;
                table.SpacingAfter = 10f;
                table.DefaultCell.Padding = 4;

                //Cell header
                cell = new PdfPCell(new Phrase("User Details", fontHeader));
                cell.PaddingBottom = 8f;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                //Data Cells
                table.AddCell(new PdfPCell(new Phrase("Email", fontText)));
                table.AddCell(new PdfPCell(new Phrase(Student.Email, fontText)));
                table.AddCell(new PdfPCell(new Phrase("Full name", fontText)));
                table.AddCell(new PdfPCell(new Phrase(Student.StudentName, fontText)));

                pdfDoc.Add(table);
            }

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

            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Item-hire-report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return RedirectToAction("Hired", new {HireId = id});
        }

        public void SendTerms(string Name, string _StudentEmail)
        {
           
            //Create Font
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
            Chunk chunk = new Chunk("Terms & Conditions", fontHeader);
            pdfDoc.Add(chunk);

            Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line);
            //Logo
            Image logo = Image.GetInstance(Server.MapPath("~/Content/Assets/images/logo.png"));
            logo.ScalePercent(15f);
            logo.SpacingBefore = 40f;
            logo.SpacingAfter = 60f;
            logo.Alignment = iTextSharp.text.Image.ALIGN_LEFT;
            pdfDoc.Add(logo); ;


            Paragraph headerTextPara = new Paragraph();
            headerTextPara.Font = fontText;

            //Header text
            string headerText = "The terms and conditions for hiring equipment can vary depending on the specific" +
                " equipment and the company providing the rental service. However, here are some general" +
                "terms and conditions that may be included in an equipment rental agreement:";

            //Footer Text
            string closeText = "These are just some of the common terms and conditions that may be included in an" +
                "equipment rental agreement. It is important to carefully review and understand all terms" +
                "and conditions before signing an agreement to rent equipment.";

            //Add Header Text
            headerTextPara.Add(headerText);
            headerTextPara.SpacingBefore = 30f;
            headerTextPara.SpacingAfter = 10f;
            pdfDoc.Add(headerTextPara);

            Paragraph closeTextPara = new Paragraph();
            closeTextPara.Font = fontText;

            Paragraph p1 = new Paragraph();
            p1.Font = fontText;
            p1.Add("1. Rental period: The rental period will be clearly defined, including the start and end date and time of the rental period. Any extensions to the rental period should be agreed upon in writing.");
            p1.SpacingBefore = 10f;
            p1.SpacingAfter = 10f;
            pdfDoc.Add(p1);

            Paragraph p2 = new Paragraph();
            p2.Font = fontText;
            p2.Add("2. Payment: The rental fee and any additional charges (such as delivery, setup, or cleaning fees) will be clearly outlined. The method of payment and any required deposit or upfront payment will also be specified.");
            p2.SpacingBefore = 10f;
            p2.SpacingAfter = 10f;
            pdfDoc.Add(p2);

            Paragraph p3 = new Paragraph();
            p3.Font = fontText;
            p3.Add("3. Equipment condition: The equipment will be rented in good working condition and will be free of defects. The renter will be responsible for inspecting the equipment before and after the rental period and reporting any damages or defects to the rental Library.");
            p3.SpacingBefore = 10f;
            p3.SpacingAfter = 10f;
            pdfDoc.Add(p3);

            Paragraph p4 = new Paragraph();
            p4.Font = fontText;
            p4.Add("4. Use of equipment: The renter will be responsible for using the equipment in a safe and appropriate manner, in accordance with any instructions or guidelines provided by the rental Library. The equipment should only be used for its intended purpose and should not be modified or altered in any way.");
            p4.SpacingBefore = 10f;
            p4.SpacingAfter = 10f;
            pdfDoc.Add(p4);

            Paragraph p5 = new Paragraph();
            p5.Font = fontText;
            p5.Add("5. Liability and insurance: The renter will be responsible for any damage or loss to the equipment during the rental period. The renter may be required to provide proof of insurance or purchase insurance coverage through the rental Library.");
            p5.SpacingBefore = 10f;
            p5.SpacingAfter = 10f;
            pdfDoc.Add(p5);

            Paragraph p6 = new Paragraph();
            p6.Font = fontText;
            p6.Add("6. Return of equipment: The renter will be responsible for returning the equipment to the rental Library at the end of the rental period, in the same condition as it was rented. Late fees may apply for equipment returned after the agreed-upon rental period.");
            p6.SpacingBefore = 10f;
            p6.SpacingAfter = 10f;
            pdfDoc.Add(p6);

            Paragraph p7 = new Paragraph();
            p7.Font = fontText;
            p7.Add("7. Termination: The rental company may terminate the rental agreement at any time if the renter breaches any of the terms and conditions, or if the equipment is used in a manner that violates any laws or regulations.");
            p7.SpacingBefore = 10f;
            p7.SpacingAfter = 10f;
            pdfDoc.Add(p7);

            Paragraph p8 = new Paragraph();
            p8.Font = fontText;
           
            p8.Add("8. Governing law: The rental agreement will be governed by the laws of the jurisdiction  in which the rental Library is located.");
            p8.SpacingBefore = 10f;
            p8.SpacingAfter = 10f;
            pdfDoc.Add(p8);

            //Add Footer Text
            closeTextPara.Add(closeText);
            closeTextPara.SpacingBefore = 30f;
            closeTextPara.SpacingAfter = 10f;
            pdfDoc.Add(closeTextPara);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();

            memoryStream.Position = 0;

            var StudentEmail = _StudentEmail;
            string Subject = "Terms & Conditions";
            string Body = "Dear " + Name +
                "</br>" +
                "Please find the attatched Terms & Conditions document" +
                "</br>" +
                "Thank you" +
                "</br>" +
                "Kindly Regards" +
                "</br>" +
                "Innovation Library";

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(ConfigurationManager.AppSettings["Email"].ToString());
            msg.To.Add(new MailAddress(StudentEmail));
            msg.Subject = Subject;
            msg.Body = Body;
            msg.IsBodyHtml = true;
            msg.Attachments.Add(new Attachment(memoryStream, "Terms-and-conditions.pdf"));
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

        public ActionResult ConfirmReturn(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            return View(_hiring);
        }

        [HttpPost]
        public ActionResult PaymentRequest(int id, string BillAmount, string Notes, 
            string IsReturnedOnTime, string IsOriginalPackaged, string IsWorking, string IsScratched,
            string IsWaterDamaged
        )
        {
            
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            if (_hiring.BillStatus == "Returned" || _hiring.BillStatus == "Waiting For Fines")
            {
                return RedirectToAction("RedundentAction");
            }
            Inspection insp = new Inspection
            {
                IsReturnedOnTime = Convert.ToBoolean(IsReturnedOnTime),
                IsOriginalPackaged = Convert.ToBoolean(IsOriginalPackaged),
                IsWorking = Convert.ToBoolean(IsWorking),
                IsScratched = Convert.ToBoolean(IsScratched),
                IsWaterDamaged = Convert.ToBoolean(IsWaterDamaged),
                HiringId = _hiring.HiringId,
                UserID = _hiring.StudentId, 
                BillAmount = BillAmount,
                Notes = Notes
            };
            _db.Inspections.Add(insp);
            _hiring.BillStatus = "Waiting For Fines";
            _db.SaveChanges();
            //SendFinesDoc(_hiring, BillAmount, Notes, insp);
            return RedirectToAction("ConfirmReturnSuccess", new { id = id });
        }

        public ActionResult DownloadInspectionReport(int id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            var HiringID = _hiring.HiringId;
            Inspection _insp = _db.Inspections.Where(ins=>ins.HiringId == HiringID).FirstOrDefault();
            if (_insp == null)
            {
                return HttpNotFound();
            }

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
            Chunk chunk = new Chunk("Inspection Feedback", fontHeader);
            pdfDoc.Add(chunk);


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
            para.Add("Hello Customer\n\nPlease find the inspection feedback on your item return.\n");

            pdfDoc.Add(para);

            //Table
            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell
            cell = new PdfPCell(new Phrase("Inspection Summary", fontHeader));
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.PaddingBottom = 8f;

            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("Is Returned On Time", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Originally Packaged", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Working", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Scratched", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Water Damaged", fontText)));


            table.AddCell(new PdfPCell(new Phrase(_insp.IsReturnedOnTime == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(_insp.IsOriginalPackaged == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(_insp.IsWorking == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(_insp.IsScratched == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(_insp.IsWaterDamaged == true ? "Yes" : "No", fontText)));

            pdfDoc.Add(table);

            //Billing Table
            table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell header
            cell = new PdfPCell(new Phrase("This amount was billed to " + _hiring.StudentEmail, fontHeader));
            cell.PaddingBottom = 8f;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            //Data Cells
            table.AddCell(new PdfPCell(new Phrase("Total price", fontText)));
            table.AddCell(new PdfPCell(new Phrase("R" + _insp.BillAmount, fontText)));

            pdfDoc.Add(table);

            //Notes From Admin
            Paragraph noteFromAdmin = new Paragraph();
            noteFromAdmin.Font = fontText;
            noteFromAdmin.Add(_insp.Notes);
            noteFromAdmin.SpacingBefore = 10f;
            noteFromAdmin.SpacingAfter = 10f;
            pdfDoc.Add(noteFromAdmin);

            //Alert
            Paragraph alert = new Paragraph();
            alert.SpacingBefore = 10f;
            alert.SpacingAfter = 10f;
            alert.Font = fontText;
            alert.Add("All payments will be made on the account details below");
            pdfDoc.Add(alert);

            //Banking Details Table
            table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell header
            cell = new PdfPCell(new Phrase("Account Details", fontHeader));
            cell.PaddingBottom = 8f;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            //Data Cells
            table.AddCell(new PdfPCell(new Phrase("Absa Branch", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Account Number", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Account Holder", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Reference", fontText)));

            table.AddCell(new PdfPCell(new Phrase("60068", fontText)));
            table.AddCell(new PdfPCell(new Phrase("935678917", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Innovation Library", fontText)));
            table.AddCell(new PdfPCell(new Phrase("935678917", fontText)));

            pdfDoc.Add(table);

            //Outro
            Paragraph Outro = new Paragraph();
            Outro.Font = fontText;
            Outro.Add("Thank you for choosing Innovation Library.\nRegards Management Team");
            pdfDoc.Add(Outro);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();

            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Ispection-Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();

            return View();
        }

        public void SendFinesDoc(Hiring hiring, string Bill, string AdminNotes, Inspection inspection)
        {
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
            Chunk chunk = new Chunk("Inspection Feedback", fontHeader);
            pdfDoc.Add(chunk);


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
            para.Add("Hello Customer\n\nPlease find the inspection feedback on your item return.\n");

            pdfDoc.Add(para);

            //Table
            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell
            cell = new PdfPCell(new Phrase("Inspection Summary", fontHeader));
            cell.Colspan = 5;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.PaddingBottom = 8f;

            table.AddCell(cell);

            table.AddCell(new PdfPCell(new Phrase("Is Returned On Time", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Originally Packaged", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Working", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Scratched", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Is Water Damaged", fontText)));


            table.AddCell(new PdfPCell(new Phrase(inspection.IsReturnedOnTime == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(inspection.IsOriginalPackaged == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(inspection.IsWorking == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(inspection.IsScratched == true ? "Yes" : "No", fontText)));
            table.AddCell(new PdfPCell(new Phrase(inspection.IsWaterDamaged == true ? "Yes" : "No", fontText)));

            pdfDoc.Add(table);

            //Billing Table
            table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell header
            cell = new PdfPCell(new Phrase("This amount was billed to " + hiring.StudentEmail, fontHeader));
            cell.PaddingBottom = 8f;
            cell.Colspan = 2;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            //Data Cells
            table.AddCell(new PdfPCell(new Phrase("Total price", fontText)));
            table.AddCell(new PdfPCell(new Phrase("R" + Bill, fontText)));

            pdfDoc.Add(table);

            //Notes From Admin
            Paragraph noteFromAdmin = new Paragraph();
            noteFromAdmin.Font = fontText;
            noteFromAdmin.Add(AdminNotes);
            noteFromAdmin.SpacingBefore = 10f;
            noteFromAdmin.SpacingAfter = 10f;
            pdfDoc.Add(noteFromAdmin);

            //Alert
            Paragraph alert = new Paragraph();
            alert.SpacingBefore = 10f;
            alert.SpacingAfter = 10f;
            alert.Font = fontText;
            alert.Add("All payments will be made on the account details below");
            pdfDoc.Add(alert);

            //Banking Details Table
            table = new PdfPTable(4);
            table.WidthPercentage = 100;
            table.HorizontalAlignment = 0;

            table.SpacingBefore = 20f;
            table.SpacingAfter = 10f;
            table.DefaultCell.Padding = 4;

            //Cell header
            cell = new PdfPCell(new Phrase("Account Details", fontHeader));
            cell.PaddingBottom = 8f;
            cell.Colspan = 4;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            //Data Cells
            table.AddCell(new PdfPCell(new Phrase("Absa Branch", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Account Number", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Account Holder", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Reference", fontText)));

            table.AddCell(new PdfPCell(new Phrase("60068", fontText)));
            table.AddCell(new PdfPCell(new Phrase("935678917", fontText)));
            table.AddCell(new PdfPCell(new Phrase("Innovation Library", fontText)));
            table.AddCell(new PdfPCell(new Phrase("935678917", fontText)));

            pdfDoc.Add(table);

            //Outro
            Paragraph Outro = new Paragraph();
            Outro.Font = fontText;
            Outro.Add("Thank you for choosing Innovation Library.\nRegards Management Team");
            pdfDoc.Add(Outro);

            pdfWriter.CloseStream = false;
            pdfDoc.Close();

            Response.Buffer = true;
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment;filename=Ispection-Report.pdf");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Write(pdfDoc);
            Response.End();
        }

        public ActionResult RedundentAction()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ConfirmReturn(int id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            if (_hiring.BillStatus == "Returned" || _hiring.BillStatus == "Waiting For Fines")
            {
                return RedirectToAction("RedundentAction");
            }
            _hiring.BillStatus = "Returned";
            _db.SaveChanges();
            return RedirectToAction("ConfirmReturnSuccess", new { id=id});
        }

        public ActionResult ConfirmReturnSuccess(int? id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            return View(_hiring);
        }

        public ActionResult ConfirmItemFine(int? id)
        {
            Hiring _hiring = _db.Hirings.Find(id);
            if (_hiring == null)
            {
                return HttpNotFound();
            }
            _hiring.BillStatus = "Returned [inc Fine Bill]";
            _db.SaveChanges();
            return RedirectToAction("ConfirmReturnSuccess", new { id = id });
        }
    }
}