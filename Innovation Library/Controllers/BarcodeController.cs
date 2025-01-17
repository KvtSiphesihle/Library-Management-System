using Aspose.BarCode.Generation;
using Innovation_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Innovation_Library.Controllers
{
    public class BarcodeController : Controller
    {
        // GET: Barcode
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(BarCode barcode)
        {
            string codeText = barcode.Text;
            string imageName = barcode.Text + "." + barcode.ImageType;
            string imagePath = "/Images/" + imageName;
            string imageServerPath = Server.MapPath("~" + imagePath);

            var encodeType = EncodeTypes.Code128;

            switch (barcode.BarcodeType)
            {
                case BarcodeType.Code128:
                    encodeType = EncodeTypes.Code128;
                    break;

                case BarcodeType.ITF14:
                    encodeType = EncodeTypes.ITF14;
                    break;

                case BarcodeType.EAN13:
                    encodeType = EncodeTypes.EAN13;
                    break;

                case BarcodeType.Datamatrix:
                    encodeType = EncodeTypes.DataMatrix;
                    break;

                case BarcodeType.Code32:
                    encodeType = EncodeTypes.Code32;
                    break;

                case BarcodeType.Code11:
                    encodeType = EncodeTypes.Code11;
                    break;

                case BarcodeType.PDF417:
                    encodeType = EncodeTypes.Pdf417;
                    break;

                case BarcodeType.EAN8:
                    encodeType = EncodeTypes.EAN8;
                    break;

                case BarcodeType.QR:
                    encodeType = EncodeTypes.QR;
                    break;
            }

            using (Stream str = new FileStream(imageServerPath, FileMode.Create, FileAccess.Write))
            {
                BarcodeGenerator gen = new BarcodeGenerator(encodeType, codeText);
                gen.Save(str, barcode.ImageType);
            }

            ViewBag.ImagePath = imagePath;
            ViewBag.ImageName = imageName;

            return View();
        }

        public ActionResult Download(string ImagePath, string ImageName)
        {
            string contentType = "application/img";
            return File(ImagePath, contentType, Path.GetFileName(ImageName));
        }
    }
}