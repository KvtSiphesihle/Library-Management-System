using Aspose.BarCode.Generation;
using Innovation_Library.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
    public class PaymentController : Controller
    {
        readonly ApplicationDbContext _db = new ApplicationDbContext();
        // GET: Payment
        public ActionResult Index()
        {
            int Id;
            if (Session["HireId"] != null)
            {
                Id = Convert.ToInt32(Session["HireId"]);
                Hiring _hiring = _db.Hirings.Find(Id);
                var StudentId = User.Identity.GetUserId();
                var Student = _db.Users.Where(u => u.Id == StudentId).FirstOrDefault();
                if (_hiring != null)
                {
                    _hiring.BillStatus = "Completed";
                    _hiring.IsPayed = true;
                    _db.SaveChanges();
                    SendTerms(Student.StudentName, _hiring.StudentEmail, _hiring);
                }
            }
            return View();
        }

        public void SendTerms(string Name, string _StudentEmail, Hiring hiring)
        {
            BarCode barcode = new BarCode();
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

            //pdfWriter.CloseStream = false;
            //pdfDoc.Close();


            barcode.BarcodeType = BarcodeType.Code128;
            barcode.ImageType = Aspose.BarCode.Generation.BarCodeImageFormat.Png;
            barcode.Text = hiring.ReferenceNo;

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

            Paragraph line1 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line1);


            //Top Heading
            Chunk chunk1 = new Chunk("Item Hire Reference Card", fontHeader);
            pdfDoc.Add(chunk1);

            Paragraph line2 = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
            pdfDoc.Add(line2);

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
            Chunk detailsHeading = new Chunk("\nReference No : " + hiring.ReferenceNo, fontHeader);
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


            table.AddCell(new PdfPCell(new Phrase(hiring.ReferenceNo, fontText)));
            table.AddCell(new PdfPCell(new Phrase(hiring.BillStatus, fontText)));
            table.AddCell(new PdfPCell(new Phrase("R" + hiring.HireTotalAmount.ToString(), fontText)));
            table.AddCell(new PdfPCell(new Phrase(hiring.HireDate, fontText)));

            pdfDoc.Add(table);

            // User Details Table

            var StudentId = hiring.StudentId;
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

            //Item Details
            int? itemID = hiring.ItemId;
            if (itemID != null)
            {
                HireItems _hireItems = _db.HireItems.Find(itemID);
                table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.HorizontalAlignment = 0;

                table.SpacingBefore = 20f;
                table.SpacingAfter = 10f;
                table.DefaultCell.Padding = 4;

                //Cell header
                cell = new PdfPCell(new Phrase("Hire item details", fontHeader));
                cell.PaddingBottom = 8f;
                cell.Colspan = 2;
                cell.VerticalAlignment = Element.ALIGN_CENTER;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                table.AddCell(cell);

                //Data Cells
                table.AddCell(new PdfPCell(new Phrase("Item Name", fontText)));
                table.AddCell(new PdfPCell(new Phrase(_hireItems.ItemName, fontText)));
                table.AddCell(new PdfPCell(new Phrase("Item Price", fontText)));
                table.AddCell(new PdfPCell(new Phrase("R" + _hireItems.ItemPrice.ToString(), fontText)));

                pdfDoc.Add(table);
            }

            //Generated DateTime
            Paragraph generatedDate = new Paragraph();
            generatedDate.Font = fontText;
            generatedDate.SpacingBefore = 20f;
            //generatedDate.SpacingAfter = 30f;
            generatedDate.Add("Generated on " + DateTime.Now.ToLongDateString());
            pdfDoc.Add(generatedDate);

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
            msg.Attachments.Add(new Attachment(memoryStream, "Hiring_Report.pdf"));
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.Timeout = 1000000;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            NetworkCredential nc = new NetworkCredential(System.Configuration.ConfigurationManager.AppSettings["Email"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Password"].ToString());
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = nc;
            smtp.Send(msg);
            msg.Dispose();

            //Response.Buffer = true;
            //Response.ContentType = "application/pdf";
            //Response.AddHeader("content-disposition", "attachment;filename=HiringReport.pdf");
            //Response.Cache.SetCacheability(HttpCacheability.NoCache);
            //Response.Write(pdfDoc);
            //Response.End();
        }
    }
}