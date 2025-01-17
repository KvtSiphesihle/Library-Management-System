using Aspose.BarCode.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class BarCode
    {
        public string Text { get; set; }

        public BarcodeType BarcodeType { get; set; }

        public BarCodeImageFormat ImageType { get; set; }
    }

    // Barcode symboligies
    public enum BarcodeType
    {
        Code128,
        Code11,
        Code32,
        QR,
        Datamatrix,
        EAN13,
        EAN8,
        ITF14,
        PDF417
    }

    // Image formats
    public enum ImageType
    {
        Png,
        Jpeg,
        Bmp,
        Emf,
        Svg
    }
}