using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Innovation_Library.Models
{
    public class CartView
    {
        public int ProdId { get; set; }
        public string ProdName { get; set; }
        public string ProdImage { get; set; }
        public int ProdQty { get; set; }
        public double ProdPrice { get; set; }
        public string Author { get; set; }
        public decimal Total { get; set; }
    }
}